using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using DTO = OriginsRx.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using ORMModel;

using OriginsRx.Models.Interfaces.Services;
using OriginsRx.Business.Services;

using System.IO;
using System.Text.RegularExpressions;

using OfficeOpenXml;
using OfficeOpenXml.Table;

using System.Diagnostics;

using OriginsRx.Azure;

namespace OriginsRx.Business.Services
{
    public class MapService : OriginsRx.Models.Interfaces.Services.IMapService
    {
        IMapper _mapper;
        ORMdbContext _db;

        public MapService(IMapper mapper, ORMdbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<DTO.MasterMaps> GetAll(DTO.StdCollectionInputs sci)
        {
            System.Linq.IQueryable<ORMModel.MasterMap> q = _db.MasterMap.Include(o => o.Columns);

            var retVal = new MasterMaps();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.MasterMaps>(retVal);

        }

        public async Task<DTO.ReturnResponse<DTO.MasterMap>> Get(long id)
        {
            var ret = new DTO.ReturnResponse<DTO.MasterMap>();

            var v = await _db.MasterMap.Include(o => o.Columns).FirstOrDefaultAsync(o => o.Id == id);
            ret.Result = _mapper.Map<DTO.MasterMap>(v);

            return ret;
        }

        public async Task<DTO.ReturnResponse<DTO.MasterMap>> Add(DTO.MasterMap inMap, long personId)
        {
            var response = new DTO.ReturnResponse<DTO.MasterMap>();

            if (inMap.Columns.Count == 0)
            {
                response.Errors.Add("Zero Columns? Really?");
            }
 
            var mmap = _mapper.Map<MasterMap>(inMap);
            OrmHelper.SetAuditColumns(ref mmap, true, personId);

            var Headers = new List<string>();
            var SqlNames = new List<string>();

            foreach (var col in mmap.Columns)
            {
                if (Headers.Contains(col.Header))
                {
                    response.Errors.Add($"Header not unique {col.Header}");
                }

                if (SqlNames.Contains(col.ColumnName))
                {
                    response.Errors.Add($"Header not unique {col.Header}");
                }

                if (response.Success)
                {
                    var tcol = col;
                    OrmHelper.SetAuditColumns(ref tcol, true, personId);
                }
            }

            if (response.Success)
            {
                try
                {
                    _db.MasterMap.Add(mmap);
                    await _db.SaveChangesAsync();

                    response.Result = _mapper.Map<DTO.MasterMap>(mmap);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }

        public Task<DTO.ReturnResponse<DTO.MasterMap>> Update(DTO.MasterMap map, long personId)
        {
            var response = new DTO.ReturnResponse<DTO.MasterMap>();

            return Task.FromResult(response);
        }

        public async Task<int> CreateDataTable(long id)
        {
            return await CreateMapDataTable(id);
        }

        public async Task<string[]> GetSheetHeaders(MemoryStream sheet)
        {

            ExcelPackage excelPackage = new ExcelPackage(sheet);

            var retval = ParseHeaders(excelPackage);
            return await Task.FromResult(retval);
        }

        public async Task<DTO.ImportPost> ImportSheet(DTO.ImportPost import)
        {
            ExcelPackage sheet = new ExcelPackage(import.Stream);
            
            var pMap = _db.PersonMap
                .Include(o => o.Columns)
                .Include(o => o.MasterMap)
                .Include(o => o.MasterMap.Columns)
                .FirstOrDefault(o => o.Id == import.PersonMapId);

            var mMap = pMap.MasterMap;

            var blobber = new Azure.Blobber();

            var bName = await blobber.Upload(import.Stream, mMap.TableName.ToLower());

            var importRec = new Import()
            {
                FileName = import.FileName,
                FileLength = import.FileLength,
                PersonMapID = import.PersonMapId,
                Status = 1,
                BlobName = bName
            };

            OrmHelper.SetAuditColumns<Import>(ref importRec, true, import.PersonId);

            _db.Import.Add(importRec);
            await _db.SaveChangesAsync();

            import.ImportId = importRec.Id;
            
            var headers = new Dictionary<string,int>();

            int sheetNo = 0;

            foreach (ExcelWorksheet worksheet in sheet.Workbook.Worksheets)
            {

                if (worksheet == null) continue;
                if (worksheet.Dimension == null) continue;

                int cols = worksheet.Dimension.Columns;
                int rows = worksheet.Dimension.Rows;

                if (cols == 0 || rows == 0) continue;

                if (rows > 0)
                {
                    for (int i = 1; i <= cols; i++)
                    {
                        //loop for headers
                        string header = worksheet.GetValue(1, i).ToString().Trim();

                        if (!headers.ContainsKey(header))
                        {
                            headers.Add(header,i);
                        }
                    }
                }

                var MTC = new Dictionary<string, string>();

                foreach(var el in pMap.Columns)
                {
                    if (headers.ContainsKey(el.TheirHeader))
                    {
                        var fieldName = mMap.Columns.First(o => o.Header == el.OurHeader).ColumnName;
                        MTC.Add(fieldName, $"{headers[el.TheirHeader]}|{el.Regex}");
                    }
                    else
                    {
                        Debug.WriteLine(el.TheirHeader);
                    }
                }

                import.SheetNo = sheetNo;
                                
                switch (mMap.TableName.ToLower())
                {
                    case "sale":
                        DoSales(worksheet, MTC,mMap,import);    
                        break;
                }

                sheetNo++;
            }


            importRec.Status = 2;

            importRec.WarnRows = import.WarnRows;
            importRec.SkipRows = import.SkipRows;

            await _db.SaveChangesAsync();

            return import;
        }

        private async void DoSales
            (
                ExcelWorksheet worksheet,
                Dictionary<string,string> colDic,
                MasterMap mMap,
                DTO.ImportPost ip
            )
        {
            
            int iStart = worksheet.Dimension.Start.Row;
            iStart = 2;

            int iEnd = GetLastRow(worksheet);

            if (iEnd > 0)
            {
 
                for (int i = iStart; i <= iEnd; i++)
                {
                    var row = worksheet.Row(i);

                    string warnings = "";
                    string requires = "";

                    var rec = new Sale();

                    foreach (string key in colDic.Keys)
                    {
                        try
                        {
                            var els = colDic[key].Split('|');

                            int ix = int.Parse(els[0]);

                            string val = worksheet.GetValue(i, ix).ToString().Trim();

                            string pattern = els[1];

                            if (!String.IsNullOrEmpty(pattern))
                            {
                                Regex regex = new Regex(pattern);
                                Match match = regex.Match(val);
                                if (match.Success)
                                {
                                    val = match.Groups[1].Value;
                                }
                            }

                            var colSpecs = mMap.Columns.First(o => o.ColumnName == key);
                    
                            if (!String.IsNullOrEmpty(val))
                            {
                                dynamic realVal = null;

                                switch (colSpecs.ColumnDataType)
                                {
                                    case 1:  
                                        realVal = val;
                                        break;
                                    case 2:  
                                        val = Regex.Replace(val, "[^0-9.]", "");
                                        var ppos = val.IndexOf('.');
                                        if (ppos != -1)
                                        {
                                            val = val.Substring(0, ppos - 1);
                                        }
                                        realVal = Convert.ToInt64(val);
                                        break;
                                    case 3:
                                        val = Regex.Replace(val, "[^0-9.]", "");
                                        realVal = Convert.ToDouble(val);
                                        break;
                                    case 4:
                                        DateTime realDT;
                                        if (DateTime.TryParse(val, out realDT))
                                        {
                                            realVal = realDT;
                                        }

                                        if (realVal == null)
                                        {
                                            long ldate = long.Parse(val);
                                            realVal = DateTime.FromOADate(ldate);
                                        }
                                        break;
                                    case 5:
                                        realVal = OrmHelper.IsTrue(val);
                                        break;
                                    case 6:
                                        val = Regex.Replace(val, "[^0-9.]", "");
                                        realVal = Decimal.Parse(val);
                                        break;
                                }

                                if (realVal != null)
                                {
                                    OrmHelper.SetPropertyValue<Sale>(ref rec, key, realVal);
                                }
                            }
                        }
                        catch
                        {
                            warnings += $"'{key}',";
                        }

                    }

                    bool isFubar = false;

                    foreach (var col in mMap.Columns)
                    {
                        if (col.Required)
                        {
                            dynamic realVal = OrmHelper.GetPropertyValue<Sale>(rec, col.ColumnName);
                            if (realVal == null)
                            {
                                requires += $"'{col.ColumnName}',";
                                isFubar = true;
                            }
                        }
                    }

                    if (!isFubar)
                    {
                        rec.ImportId = ip.ImportId;
                        rec.Row = i;
                        rec.Sheet = ip.SheetNo;
                       
                        OrmHelper.SetAuditColumns<Sale>(ref rec, true, ip.PersonId);

                        _db.Sale.Add(rec);
                    }

                    if (isFubar || warnings.Length > 0)
                    {
                        if (isFubar) ip.SkipRows++;
                        if (warnings.Length > 0) ip.WarnRows++;

                        string errInfo = "{";
                        errInfo += "warns:[" + warnings.TrimEnd(',') + "],";
                        errInfo += "skips:[" + requires.TrimEnd(',') + "]";
                        errInfo += "}";

                        var ie = new ImportError()
                        {
                            ImportId = ip.ImportId,
                            Status = isFubar ? 1 : 0,
                            Row = i,
                            Sheet = ip.SheetNo,
                            Errors = errInfo
                        };

                        OrmHelper.SetAuditColumns<ImportError>(ref ie, true, ip.PersonId);

                        _db.ImportError.Add(ie);
                    }
                }

            }
        }

        private string[] ParseHeaders(ExcelPackage sheet)
        {
            var headers = new List<string>();

            foreach (ExcelWorksheet worksheet in sheet.Workbook.Worksheets)
            {

                if (worksheet == null) continue;
                if (worksheet.Dimension == null) continue;
                
                int cols = worksheet.Dimension.Columns;
                int rows = worksheet.Dimension.Rows;

                if (cols == 0 || rows == 0) continue;

                if (rows > 0)
                {
                    for (int i = 1; i <= cols; i++)
                    {
                        //loop for headers
                        string header = worksheet.GetValue(1, i).ToString().Trim();

                        if (!headers.Contains(header))
                        {
                            headers.Add(header);
                        }
                    }
                }
            }

            return headers.ToArray();
        }

        private int GetLastRow(ExcelWorksheet worksheet)
        {
            int colB = 2;
            int colE = worksheet.Dimension.End.Column;

            int lastRow = worksheet.Dimension.End.Row;
            while (lastRow >= 1)
            {
                var range = worksheet.Cells[lastRow, colB, lastRow, colE];
                if (range.Any(c => c.Value != null))
                {
                    break;
                }
                lastRow--;
            }

            return lastRow;
        }

        private ExcelPackage ReadSheetFromDisk(string fullFileName)
        {
            var fileInfo = new FileInfo(fullFileName);
            if (fileInfo.Exists)
            {
                var package = new ExcelPackage(fileInfo);

                return package;
            }

            return null;
        }

        private async Task<int> CreateMapDataTable(long masterMapId)
        {
            var master = _db.MasterMap.FirstOrDefault(o => o.Id == masterMapId);

            if (master.Locked == true) return 0;
            master.Locked = true;
            _db.SaveChanges();

            string sql = $"Create Table dbo.{master.TableName} (";

            string mods = "";

            sql += "Id bigint NOT NULL IDENTITY(1,1) PRIMARY KEY, ";
            sql += "ImportId bigint NOT NULL, ";
            sql += "Sheet bigint NOT NULL, ";
            sql += "Row bigint NOT NULL, ";

            var columns = _db.MasterMapColumn.Where(o => o.MasterMapID == masterMapId).OrderBy(o => o.Id);
            foreach (var col in columns)
            {
                var cName = col.ColumnName;
                var oName = cName;

                cName = cName.Replace('|', '_');
                cName = cName.Replace('-', '_');
                cName = cName.Replace('!', '_');
                cName = cName.Replace('$', '_');
                cName = cName.Replace('&', '_');
                cName = cName.Replace('*', '_');
                cName = cName.Replace('%', '_');
                cName = cName.Replace('(', '_');
                cName = cName.Replace(')', '_');
                cName = cName.Replace(' ', '_');
                cName = cName.Replace("#" ,"_num");

                if (cName != oName)
                {
                    col.ColumnName = cName;
                    _db.SaveChanges();
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
                    case 6:
                        sql += "decimal(18,2) ";
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

/*
                if (cName.EndsWith("Id"))
                {
                    var tablename = cName.Substring(0, cName.Length - 2);
                    // validate table exists?? for now assume it does
                    var fk = $"ALTER TABLE dbo.{master.TableName} WITH CHECK ADD FOREIGN KEY({cName}) REFERENCES dbo.{tablename} (Id);";
                    mods += fk;
                }
*/
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

            await _db.Database.ExecuteSqlCommandAsync(sql);

            master.Locked = true;
            _db.SaveChanges();

            return 1;
        }
    }
}
