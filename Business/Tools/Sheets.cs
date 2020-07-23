using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using DTO = OriginsRx.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using ORMModel;

//https://github.com/FairSite2C/EPPlus

namespace OriginsRx.Business.Tools
{
    public class Sheets
    {
  
        public string[] ParseHeaders(ExcelPackage sheet)
        {
            var headers = new List<string>();

            foreach (ExcelWorksheet worksheet in sheet.Workbook.Worksheets)
            {
            
                int cols = worksheet.Dimension.Columns;
                int rows = worksheet.Dimension.Rows;
             
                if (rows > 0)
                {
                    for (int i = 1; i <= cols; i++)
                    {
                        //loop for headers
                        string header = worksheet.Cells[1,i].Value.ToString().Trim();

                        if (!headers.Contains(header))
                        {
                            headers.Add(header);
                        }
                    }
                }
            }

            return headers.ToArray();
        }

        public ExcelPackage ReadSheetFromDisk(string fullFileName)
        {
            var fileInfo = new FileInfo(fullFileName);
            if (fileInfo.Exists)
            {
                var package = new ExcelPackage(fileInfo);

                return package;
            }

            return null;
        }

        public void CreateMapDataTable(long masterMapId)
        {

            using (var db = Global.GetDb())
            {
                
                var master = db.MasterMap.FirstOrDefault(o => o.Id == masterMapId);

                if (master.Locked == true) return;

                string sql = $"Create Table dbo.{master.TableName} (";

                string mods = "";

                sql += "Id bigint NOT NULL IDENTITY(1,1) PRIMARY KEY, ";
                sql += "ImportId bigint NOT NULL, ";
                sql += "Sheet bigint NOT NULL, ";
                sql += "Row bigint NOT NULL, ";

                var columns = db.MasterMapColumn.Where(o => o.MasterMapID == masterMapId).OrderBy(o => o.Id);
                foreach(var col in columns)
                {
                    var colChanged = false;
                    var cName = col.ColumnName;
                    var oName = cName;

                    cName = cName.Replace('|', '_');
                    cName = cName.Replace('-', '_');
                    cName = cName.Replace('!', '_');
                    cName = cName.Replace(' ', '_');

                    if (cName != oName)
                    {
                        colChanged = true;
                        col.ColumnName = cName;
                    }

                    sql += $"{cName} ";

                    switch (col.ColumnDataType)
                    {
                        case 1:
                            var len = col.MaxLength == null || col.MaxLength == 0 ? 100 : col.MaxLength;
                            sql += $"nvarchar({len}) ";
                            break;
                        case 2:
                            sql += "bigint ";
                            break;
                        case 3:
                            sql += "float ";
                            break;
                        case 4:
                            sql += "datetime ";
                            break;
                        case 5:
                            sql += "bit ";
                            break;
                    }

                    if (col.Required)
                    {
                        sql += "NOT NULL";

                    }

                    sql += $", ";
 
                    if (col.MakeIndex)
                    {
                         var ix = $"CREATE INDEX idx_{cName } ON dbo.{master.TableName} ({cName});";
                         mods += ix;
                    }

                    if (cName.EndsWith("Id"))
                    {
                        var tablename = cName.Substring(0, cName.Length - 2);
                        // validate table exists?? for now assume it does
                        var fk = $"ALTER TABLE dbo.{master.TableName} WITH CHECK ADD FOREIGN KEY({cName}) REFERENCES dbo.{tablename} (Id);";
                        mods += fk;
                    }

                    if (colChanged) db.SaveChanges();

                }

                sql += "CreateDT datetime DEFAULT GETDATE(),";
                sql += "UpdateDT datetime DEFAULT GETDATE(),";
                sql += "CreateBy bigint NULL,";
                sql += "UpdateBy bigint NULL,";
                sql += "Deleted bit DEFAULT 0";
                sql += ");";   
                var ixa = $"CREATE INDEX idx_importId ON dbo.{master.TableName} (ImportId,row);";
                mods += ixa;

                sql += mods;

                db.Database.ExecuteSqlCommand(sql);

                master.Locked = true;
                db.SaveChanges();

            }
        }
    }
}
