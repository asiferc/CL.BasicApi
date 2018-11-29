using AutoMapper;

using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.Mapping;
using Cardlytics.BasicApi.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using StructureMap;

using Swashbuckle.AspNetCore.Swagger;

namespace Cardlytics.BasicApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            services
                .AddApiVersioning(o =>
                {
                    o.ReportApiVersions = true;
                    o.Conventions.Add(new VersionByNamespaceConvention());
                });
            services
                .AddSwaggerGen(opt =>
                {
                    opt.SwaggerDoc("v1", new Info { Title = "Institutions Api", Version = "v1" });
                });
            services
                .AddDbContext<BasicDbContext>(
                    opt => opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0"));
        }

        public void ConfigureContainer(Registry registry)
        {
            registry.Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
                scanner.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // register middleware specific order
            app.UseRequestLogMiddleware()
                .UseExceptionCaptureMiddleware();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Institutions API");
            });

            // Set up Mappings
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperConfig>());
        }
    }
}
