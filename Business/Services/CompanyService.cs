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

namespace OriginsRx.Business.Services
{
    public class CompanyService : OriginsRx.Models.Interfaces.Services.ICompanyService
    {
        IMapper _mapper;
        ORMdbContext _db;

        public CompanyService(IMapper mapper, ORMdbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<DTO.Companies> GetAll(DTO.StdCollectionInputs sci)
        {
            System.Linq.IQueryable<ORMModel.Company> q = _db.Company;

            var retVal = new Companies();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Companies>(retVal);
        }

        public async Task<DTO.ReturnResponse<DTO.Company>> Get(long id)
        {
            var ret = new DTO.ReturnResponse<DTO.Company>();

            var v = await _db.Company.FirstOrDefaultAsync(o => o.Id == id);
            ret.Result = _mapper.Map<DTO.Company>(v);

            return ret;
        }

        public async Task<DTO.ReturnResponse<DTO.Company>> Add(DTO.CompanyAdd inVal)
        {
            var ret = new DTO.ReturnResponse<DTO.Company>();

            var dbVal = _mapper.Map<Company>(inVal);

            OrmHelper.SetAuditColumns<Company>(ref dbVal, true, 1);
            _db.Company.Add(dbVal);

            await _db.SaveChangesAsync();

            ret.Result = _mapper.Map<DTO.Company>(dbVal);

            return ret;
        }

        public async Task<DTO.ReturnResponse<DTO.Company>> Update(DTO.CompanyMod company)
        {
            var ret = new DTO.ReturnResponse<DTO.Company>();

            var v = await _db.Company.FirstOrDefaultAsync(o => o.Id == company.Id);

            v = _mapper.Map<Company>(company);
            await _db.SaveChangesAsync();

            ret.Result = _mapper.Map<DTO.Company>(v);
            return ret;
        }

        public async Task<DTO.Persons> GetAllPersons(DTO.StdCollectionInputsId sci)
        {
            
            System.Linq.IQueryable<ORMModel.Person> q = _db.Person.Where( o => o.CompanyId == sci.Id);

            var retVal = new Persons();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Persons>(retVal);
        }

        public async Task<DTO.PersonMaps> GetAllPersonMaps(DTO.StdCollectionInputsId sci)
        {
            var list =  (List<DTO.SearchParameter>) sci.SearchParameters;
            long mmapId =list.First(o => o.Column.ToLower() == "mastermapid").Value;
            System.Linq.IQueryable<ORMModel.PersonMap> q =  
                _db.PersonMap
                .Include( o => o.Columns)
                .Include(o => o.Person)
                .Where(o => o.MasterMapId == mmapId && o.Person.CompanyId == sci.Id)
            ;

            var retVal = new PersonMaps();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.PersonMaps>(retVal);
        }
    }

}
