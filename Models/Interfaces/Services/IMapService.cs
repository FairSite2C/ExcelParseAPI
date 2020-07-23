using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OriginsRx.Models.DTOs;

namespace OriginsRx.Models.Interfaces.Services
{
    public interface IMapService
    {
        Task<MasterMaps> GetAll(StdCollectionInputs sci);

        Task<ReturnResponse<MasterMap>> Get(long id);

        Task<ReturnResponse<MasterMap>> Add(MasterMap map, long personId);

        Task<ReturnResponse<MasterMap>> Update(MasterMap map, long personId);

        Task<int> CreateDataTable(long id);

        Task<string[]> GetSheetHeaders( MemoryStream sheet);

        Task<ImportPost> ImportSheet(ImportPost import);
    }
}
