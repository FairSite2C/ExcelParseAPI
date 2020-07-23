using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using AutoMapper.QueryableExtensions;

using DTO = OriginsRx.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using ORMModel;

using OriginsRx.Models.Interfaces.Services;
using OriginsRx.Business.Services;

namespace OriginsRx.Business.Services
{
    public class PersonService : OriginsRx.Models.Interfaces.Services.IPersonService
    {
        IMapper _mapper;
        ORMdbContext _db;

        public PersonService(IMapper mapper, ORMdbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<DTO.Persons> GetAll(DTO.StdCollectionInputs sci)
        {
            System.Linq.IQueryable<ORMModel.Person> q = _db.Person;

            var retVal = new Persons();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Persons>(retVal);
        }

        public async Task<DTO.ReturnResponse<DTO.Person>> Get(long id)
        {
            var ret = new DTO.ReturnResponse<DTO.Person>();
            
            var v = await _db.Person.FirstOrDefaultAsync(o => o.Id == id);
            ret.Result = _mapper.Map<DTO.Person>(v);

            return ret;
        }

        public async Task<long> GetMe(string email)
        {
 
            var v = await _db.Person.FirstOrDefaultAsync(o => o.Email == email);
            return v.Id;
        }

        public async Task<DTO.ReturnResponse<DTO.Person>> Add(DTO.Person Person)
        {
            var ret = new DTO.ReturnResponse<DTO.Person>();

            var val = _mapper.Map<Person>(Person);
            OrmHelper.SetAuditColumns<Person>(ref val, true, 1);
            _db.Person.Add(val);
            await _db.SaveChangesAsync();

            ret.Result = _mapper.Map<DTO.Person>(val);
            return ret;
        }

        public async Task<DTO.ReturnResponse<DTO.Person>> Update(DTO.Person Person)
        {
            var ret = new DTO.ReturnResponse<DTO.Person>();

            var v = await _db.Person.FirstOrDefaultAsync(o => o.Id == Person.Id);

            v = _mapper.Map<Person>(Person);
            await _db.SaveChangesAsync();

            ret.Result =  _mapper.Map<DTO.Person>(v);

            return ret;
        }
                
        public async Task<DTO.PersonMaps> GetAllMaps(DTO.StdCollectionInputsId sci, long personId)
        {
            System.Linq.IQueryable<ORMModel.PersonMap> q =
            _db.PersonMap.Include(o => o.Columns).Where(o => o.MasterMapId == sci.Id && o.PersonId == personId);

            var retVal = new PersonMaps();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.PersonMaps>(retVal);
        }

        public async Task<DTO.ReturnResponse<DTO.PersonMap>> GetMap(long id)
        {
            var ret = new DTO.ReturnResponse<DTO.PersonMap>();

            var v = await _db.PersonMap.Include(o => o.Columns).FirstOrDefaultAsync(o => o.Id == id);
            ret.Result =  _mapper.Map<DTO.PersonMap>(v);

            return ret;
        }

        public async Task<DTO.ReturnResponse<DTO.PersonMap>> AddMap(DTO.PersonMapAdd map, long personId)
        {
            var response = new DTO.ReturnResponse<DTO.PersonMap>();

            if (map.Columns.Count == 0)
            {
                response.Errors.Add("Zero Columns? Really?");
            }

            var mmap = _db.MasterMap.Include(o => o.Columns).First(o => o.Id == map.MasterMapId);
            if (mmap == null)
            {
                response.Errors.Add($"Invalid MasterMapID {map.MasterMapId}");
            }

            var pmap = _mapper.Map<PersonMap>(map);
            OrmHelper.SetAuditColumns(ref pmap, true, personId);

            foreach (var col in pmap.Columns)
            {
                if (mmap.Columns.First(o => o.Header == col.OurHeader) == null)
                {
                    response.Errors.Add($"header does not exist {col.OurHeader}");

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
                    _db.PersonMap.Add(pmap);
                    await _db.SaveChangesAsync();

                    response.Result = _mapper.Map<DTO.PersonMap>(pmap);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }

        public async Task<DTO.ReturnResponse<DTO.PersonMap>> UpdateMap(DTO.PersonMapMod inMap, long personId)
        {
            var response = new DTO.ReturnResponse<DTO.PersonMap>();

            var map = await _db.PersonMap.Include(o => o.Columns).FirstOrDefaultAsync(o => o.Id == inMap.Id);

            if (map == null)
            {
                response.Errors.Add($"Person Map {inMap.Id} does not exist");

            }
            else if (map.Locked)
            {

                response.Errors.Add($"Person Map {inMap.Id} is locked, no changes allowed");

            }
            else
            {

            }

            return response;
        }
    }
}