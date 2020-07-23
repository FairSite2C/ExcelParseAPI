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
    public class SaleService : OriginsRx.Models.Interfaces.Services.ISaleService
    {
        IMapper _mapper;
        ORMdbContext _db;

        public SaleService(IMapper mapper, ORMdbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<DTO.Sales> GetAll(DTO.StdCollectionInputs sci)
        {
            System.Linq.IQueryable<ORMModel.Sale> q = _db.Sale;

            var c = new Sales();

            await c.Run(_mapper, q, sci);

            var retVal = _mapper.Map<DTO.Sales>(c);

            return retVal;
        }

        public async Task<DTO.Sales> GetAllCompany(DTO.StdCollectionInputsId sci)
        {

            System.Linq.IQueryable<ORMModel.Sale> q = 
                _db.Sale.Include(o => o.Import.Map.Person)
                .Where(o => o.Import.Map.Person.CompanyId == sci.Id);

            var retVal = new Sales();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Sales>(retVal);
        }

        public async Task<DTO.Sales> GetAllPerson(DTO.StdCollectionInputsId sci)
        {

            System.Linq.IQueryable<ORMModel.Sale> q =
                _db.Sale.Include(o => o.Import.Map.Person)
                .Where(o => o.Import.CreateBy == sci.Id);

            var retVal = new Sales();

            await retVal.Run(_mapper, q, sci);

            return _mapper.Map<DTO.Sales>(retVal);
        }
        
        public async Task<DTO.ReturnResponse<DTO.Sale>> Get(long id)
        {
            var ret = new DTO.ReturnResponse<DTO.Sale>();

            var v = await _db.Company.FirstOrDefaultAsync(o => o.Id == id);
            ret.Result = _mapper.Map<DTO.Sale>(v);

            return ret;
        }
    }
}