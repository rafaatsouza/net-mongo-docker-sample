using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.OpenApi.Models;

namespace MongoDocker.Sample.Ui.Api
{
    public class Startup
    {
        private const string ApiXmlDocumentationName = "MongoDocker.Sample.Ui.Api.xml";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.AddSwaggerGen(sa =>
            {
                sa.SwaggerDoc("v1",
                    new OpenApiInfo()
                    {
                        Title = "Quotes Mock Web API",
                        Version = "v1",
                        Description = "Web API available to check stock action prices"
                    });

                sa.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ApiXmlDocumentationName));
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
