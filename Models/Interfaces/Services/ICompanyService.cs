using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OriginsRx.Models.DTOs;

namespace OriginsRx.Models.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<Companies> GetAll(StdCollectionInputs sci);

        Task<ReturnResponse<Company>> Get(long id);

        Task<ReturnResponse<Company>> Add(CompanyAdd company);

        Task<ReturnResponse<Company>> Update(CompanyMod company);

        Task<Persons> GetAllPersons(StdCollectionInputsId sci);

        Task<PersonMaps> GetAllPersonMaps(StdCollectionInputsId sci);
    }
}
