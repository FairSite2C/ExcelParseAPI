using System;
using System.Collections.Generic;
using System.Text;
using ORMModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OriginsRx
{
    public static class Global
    {
        public static IConfiguration config;

        public static ORMdbContext GetDb()
        {
            //var dbConnectionString = OriginsRx.Global.config["dbConnect"];
            var dbConnectionString = "Server=tcp:originsrx.database.windows.net,1433; Initial Catalog=OriginsRx;Persist Security Info=False; User ID=boss; Password=OriginsRx!; MultipleActiveResultSets=False; Encrypt=True; TrustServerCertificate=False; Connection Timeout=30;";

            var options = new DbContextOptionsBuilder<ORMdbContext>()
                .UseSqlServer(dbConnectionString, providerOptions => providerOptions.CommandTimeout(60))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
             .Options;

            return new ORMdbContext(options);
        }
    }

}
