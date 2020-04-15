using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDockerSample.Infrastructure.Repository;
using MongoDockerSample.Ui.Api.Middlewares;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

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

            app.UseMvc();
        }
    }
}