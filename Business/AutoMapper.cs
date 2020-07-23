using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using EF = ORMModel;
using DTO = OriginsRx.Models.DTOs;

namespace OriginsRx.Business
{
    public class AutoMapper
    {
        public static IMapper GetAutoMapper()
        {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.ValidateInlineMaps = false;

                cfg.CreateMap<DTO.PersonMapAdd, EF.PersonMap>();
                cfg.CreateMap<DTO.PersonMapColumnAdd, EF.PersonMapColumn>();

                cfg.CreateMap<EF.Company, DTO.Company>();
                cfg.CreateMap<EF.Import, DTO.Import>();
                cfg.CreateMap<EF.ImportError, DTO.ImportError>();
                cfg.CreateMap<EF.MasterMap, DTO.MasterMap>();
                cfg.CreateMap<EF.MasterMapColumn, DTO.MasterMapColumn>();
                cfg.CreateMap<EF.Person, DTO.Person>();
                cfg.CreateMap<EF.PersonMap, DTO.PersonMap>();
                cfg.CreateMap<EF.PersonMapColumn, DTO.PersonMapColumn>();
                cfg.CreateMap<EF.Sale, DTO.Sale>();
            });

            IMapper mapper = config.CreateMapper();

            return mapper;

        }
    }
}
