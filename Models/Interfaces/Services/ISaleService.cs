using System;
using System.Collections.Generic;
using System.Text;
using OriginsRx.Models.DTOs;
using System.Threading.Tasks;

namespace OriginsRx.Models.Interfaces.Services
{
    public interface ISaleService
    {
        Task<Sales> GetAll(StdCollectionInputs sci);

        Task<Sales> GetAllCompany(StdCollectionInputsId sci);

        Task<Sales> GetAllPerson(StdCollectionInputsId sci);

        Task<ReturnResponse<Sale>> Get(long id);
    }
}
