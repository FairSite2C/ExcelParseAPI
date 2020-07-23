using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OriginsRx.Models.DTOs;

namespace OriginsRx.Models.Interfaces.Services
{
    public interface IImportServicee
    {
        Task<ReturnResponse<Import>> Get(long id);

        Task<Imports> GetAllByPersonMap(StdCollectionInputsId sc);

        Task<Imports> GetAllByMasterMap(StdCollectionInputsId sc);

        Task<Imports> GetAllByPerson(StdCollectionInputsId sc);

        Task<Imports> GetAllByCompany(StdCollectionInputsId sc);

        Task<ImportPost> ImportSheet(ImportPost import);

        Task<string[]> GetSheetHeaders(MemoryStream sheet);

    }
}
