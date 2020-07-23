using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

using Microsoft.Extensions.Configuration;
using  cxz

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Authorization;

using System.Security.Claims;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.IO;
using System.Reflection;

using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;


using ORMModel;
using OriginsRx.Models.Interfaces.Services;
using OriginsRx.Business.Services;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace OriginsRx
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            config = configuration;
            Global.config = configuration;
        }

        public IConfiguration config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllAccess",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllAccess"));
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddAzureAdBearer(options => config.Bind("AzureAd", options));

            /*
            services.AddMvc().AddJsonOptions(opt =>
            {
                if (opt.SerializerSettings.ContractResolver != null)
                {
                    var resolver = opt.SerializerSettings.ContractResolver as DefaultContractResolver;
                    resolver.NamingStrategy = null;
                }
            });
            */

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
             
            //    https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "OriginsRx API",
                    Description = "List of Magical Mysteries",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Ahmed Mustapha Salem",
                        Email = string.Empty,
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper();

            IMapper mapper = OriginsRx.Business.AutoMapper.GetAutoMapper();
            services.AddSingleton<IMapper>(mapper);

            //https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext
            var dbConnectionString = config["dbConnect"];

            services.AddDbContext<ORMdbContext>(
            options =>
            options
            
                .UseSqlServer(dbConnectionString, providerOptions => providerOptions.CommandTimeout(60))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                ,

                ServiceLifetime.Transient
            );

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IPersonService,  PersonService>();
            services.AddScoped<IMapService,     MapService>();
            services.AddScoped<IImportServicee, ImportService>();
            services.AddScoped<ISaleService,    SaleService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseCors("AllowAllAccess");

            /*
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:2020"));
  
            app.UseCors(builder =>
                builder.WithOrigins("https://originsrx-front.azurewebsites.net"));
            */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

//            app.UseStaticFiles();

//            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
