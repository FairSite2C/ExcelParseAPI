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
    public class ImportService : OriginsRx.Models.Interfaces.Services.IImportServicee
    {
        IMapper _mapper;
        ORMdbContext _db;

        public ImportService(IMapper mapper, ORMdbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<DTO.Imports> GetAllByPerson(DTO.StdCollectionInputsId sci)
        {
            System.Linq.IQueryable<ORMModel.Import> q = _db.Import
                .Include(o => o.Map)
                .Where(o => o.Map.Person.Id == sci.Id);

            var retVal = new Imports();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Imports>(retVal);

        }

        public async Task<DTO.Imports> GetAllByCompany(DTO.StdCollectionInputsId sci)
        {
            System.Linq.IQueryable<ORMModel.Import> q = _db.Import
                .Include(o => o.Map.Person)
                .Where(o => o.Map.Person.CompanyId == sci.Id);

            var retVal = new Imports();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Imports>(retVal);
        }

        public async Task<DTO.Imports> GetAllByPersonMap(DTO.StdCollectionInputsId sci)
        {
            System.Linq.IQueryable<ORMModel.Import> q = _db.Import
                .Where(o => o.PersonMapID == sci.Id);

            var retVal = new Imports();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Imports>(retVal);

        }

        public async Task<DTO.Imports> GetAllByMasterMap(DTO.StdCollectionInputsId sci)
        {
            System.Linq.IQueryable<ORMModel.Import> q = _db.Import
                .Include(o => o.Map)
                .Where(o => o.Map.MasterMapId == sci.Id);

            var retVal = new Imports();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Imports>(retVal);

        }

        public async Task<DTO.ReturnResponse<DTO.Import>> Get(long id)
        {
            var response = new DTO.ReturnResponse<DTO.Import>();

            var v = await _db.Import.Include(o => o.ImportErrors).FirstOrDefaultAsync(o => o.Id == id);

            response.Result = _mapper.Map<DTO.Import>(v);

            return response;
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

            var headers = new Dictionary<string, int>();

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
                            headers.Add(header, i);
                        }
                    }
                }

                var MTC = new Dictionary<string, string>();

                foreach (var el in pMap.Columns)
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
                        DoSales(worksheet, MTC, mMap, import);
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

        private void DoSales
            (
                ExcelWorksheet worksheet,
                Dictionary<string, string> colDic,
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

        public async Task<string[]> GetSheetHeaders(MemoryStream sheet)
        {

            ExcelPackage excelPackage = new ExcelPackage(sheet);

            var retval = ParseHeaders(excelPackage);
            return await Task.FromResult(retval);
        }

        private string[] ParseHeaders(ExcelPackage sheet)
        {
            var headers = new List<string>();

            foreach (ExcelWorksheet worksheet in sheet.Workbook.Worksheets)
            {

                if (worksheet == null) continue;
                if (worksheet.Dimension == null) continue;

                int cols = worksheet.Dimension.Columns;
                int rows = GetLastRow( worksheet);

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
    }
}
