using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OriginsRx.Models.DTOs;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

using DTO = OriginsRx.Models.DTOs;
using EF = ORMModel;
using System.Linq.Expressions;

using AutoMapper;

namespace OriginsRx.Business
{
    public class BaseCollection<TDTO, TEF> : CollectionResponse
    {
        public async Task Run(IMapper mapper, IQueryable<TEF> q, DTO.StdCollectionInputs sci)
        {
            var tq = q;
           
            if (!sci.IncludeDeleted)
            {
                tq = OrmHelper.EqualToProperty<TEF>(tq, "Deleted", false);
            }

            tq = AddSearchParameters(tq, sci);

            TotalCount = tq.Count();

            string sIncludeDeleted = sci.IncludeDeleted ? "true" : "false";

            string QM = sci.Route.IndexOf("?") == -1 ? "?" : "&";

            q = AddSearchParameters(q, sci);

            q = OrmHelper.AddStdCollectionParameters<TEF>(q, sci.Offset, sci.Limit, sci.IncludeDeleted, 
                sci.Sort);

            Items = mapper.Map<TDTO[]>(await q.ToArrayAsync());

            ItemCount = Items.Count();

            Paging.Self =  $"{sci.Offset}";
            Paging.First = $"0";

            if (sci.Limit < TotalCount && sci.Limit > 0)
            {
                if (sci.Offset > 0)
                {
                    Paging.Prev = $"{(sci.Offset > 0 ? sci.Offset - sci.Limit : 0)}";
                }

                if (sci.Offset < TotalCount)
                {
                    Paging.Next = $"{(sci.Offset + sci.Limit)}";

                    Paging.Last = $"{(TotalCount - sci.Limit)}";

                }

                if (sci.Offset + sci.Limit >= TotalCount)
                {
                    Paging.Next = null;
                }
            }
        }

        public IEnumerable<TDTO> Items { get; set; }

        private IQueryable<TEF> AddSearchParameters(IQueryable<TEF> q, DTO.StdInputs sci)
        {
       
            if (sci.SearchParameters != null && sci.SearchParameters.Count > 0)
            {
                int count = sci.SearchParameters.Count;

                dynamic expAll = null;
                bool orJoiner = false;

                for (int i = 0; i < count; i++)
                {
                    var sp = sci.SearchParameters.ElementAt(i);

                    var exp = OrmHelper.GetBinaryExpression<TEF>(sp.Column, (int)sp.Compare, sp.Value);
                 
                    if (i == 0)
                    {
                        expAll = exp;
                    }
                    else
                    {
                        expAll = orJoiner ? OrmHelper.ExpressionOr(expAll, exp) : 
                                            OrmHelper.ExpressionAnd(expAll, exp);
                    }

                    orJoiner = sp.Or;
                }

                return OrmHelper.WhereWrapper<TEF>(q, expAll);
            }

            return q;
        }
    }


    public class MasterMaps : BaseCollection<DTO.MasterMap, EF.MasterMap>
    {
    }

    public class Companies : BaseCollection<DTO.Company, EF.Company>
    {
    }

    public class Persons : BaseCollection<DTO.Person, EF.Person>
    {
    }

    public class PersonMaps : BaseCollection<DTO.PersonMap, EF.PersonMap>
    {
    }

    public class Sales : BaseCollection<DTO.Sale, EF.Sale>
    {
    }

    public class Imports : BaseCollection<DTO.Import, EF.Import>
    {
    }

}
