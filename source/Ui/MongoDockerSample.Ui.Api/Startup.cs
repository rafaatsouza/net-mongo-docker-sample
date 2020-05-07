using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDockerSample.Infrastructure.Repository;
using MongoDockerSample.Ui.Api.Middlewares;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MongoDockerSample.Ui.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var repositoryConfiguration = Configuration.GetSection("RepositoryConfiguration").Get<RepositoryConfiguration>();

            services.AddRepository(repositoryConfiguration);
            services.AddServices();

            services
                .AddMvc(m =>
                {
                    m.EnableEndpointRouting = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddSwaggerGen(s =>
            {
                var apiInfo = new OpenApiInfo()
                {
                    Title = "MongoDocker Web API",
                    Version = "v1",
                    Description = ".NET Core Web API created to simulate simple MongoDb interaction"
                };

                s.SwaggerDoc("v1", apiInfo);
                s.SchemaFilter<SwaggerDefaultSchemaFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}