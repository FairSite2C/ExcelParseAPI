using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OriginsRx.Models.DTOs;

namespace OriginsRx.Models.Interfaces.Services
{
    public interface IPersonService
    {
        Task<Persons> GetAll(StdCollectionInputs sci);

        Task<ReturnResponse<Person>> Get(long id);

        Task<long> GetMe(string email);

        Task<ReturnResponse<Person>> Add(Person person);

        Task<ReturnResponse<Person>> Update(Person person);

        Task<PersonMaps> GetAllMaps(StdCollectionInputsId sci, long personId);

        Task<ReturnResponse<PersonMap>> GetMap(long id);

        Task<ReturnResponse<PersonMap>> AddMap(PersonMapAdd map, long personId);

        Task<ReturnResponse<PersonMap>> UpdateMap(PersonMapMod map, long personId);

    }
}
