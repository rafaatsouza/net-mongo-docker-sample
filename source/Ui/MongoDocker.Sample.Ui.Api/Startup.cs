using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using MongoDocker.Sample.Domain.Service.Interfaces;
using MongoDocker.Sample.Infrastructure.Provider;
using MongoDocker.Sample.Domain.Contract.DTO;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Logging;
using MongoDocker.Sample.Ui.Api.Middlewares;

namespace MongoDocker.Sample.Ui.Api
{
    public class Startup
    {
        private const string ContractXmlDocumentationName = "MongoDocker.Sample.Domain.Contract.xml";
        private const string ServiceXmlDocumentationName = "MongoDocker.Sample.Domain.Service.xml";
        private const string InfraXmlDocumentationName = "MongoDocker.Sample.Infrastructure.Provider.xml";
        private const string ApiXmlDocumentationName = "MongoDocker.Sample.Ui.Api.xml";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var mongoDbConfiguration = Configuration.GetSection("MongoDb").Get<MongoDbConfigurationValues>();

            services.AddSingleton(mongoDbConfiguration);
            services.AddSingleton<IMongoDbService, MongoDbService>();

            services
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.AddLogging(options =>
            {
                options.AddSeq(Configuration.GetSection("LogSeq"));
            });

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",
                    new Info()
                    {
                        Title = "MongoDocker Web API",
                        Version = "v1",
                        Description = ".NET Core Web API created to simulate simple MongoDb interaction"
                    });

                s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ContractXmlDocumentationName));
                s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ServiceXmlDocumentationName));
                s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, InfraXmlDocumentationName));
                s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ApiXmlDocumentationName));
            });
        }

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

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}