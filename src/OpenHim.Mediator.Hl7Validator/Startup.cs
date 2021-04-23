using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenHim.Mediator.HL7Validator.Configuration;
using WcPhdc.OpenHim.Mediator.Extensions;
using OpenHim.Mediator.HL7Validator.Services;
using Serilog;
using System.Text.Json;
using WcPhdc.OpenHim.Mediator.Services;

namespace OpenHim.Mediator.HL7Validator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                var jsonOpenHimFormatter = new SystemTextJsonOutputFormatter(new JsonSerializerOptions());
                // Need to add support for the media type expected by openHIM otherwise dotnet will return a 406.
                jsonOpenHimFormatter.SupportedMediaTypes.Add("application/json+openhim");

                options.OutputFormatters.Add(jsonOpenHimFormatter);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenHim.Mediator.HL7Validator", Version = "v1" });
            });

            services.AddOpenHimMediator(Configuration.GetSection("mediatorconfig"));

            services.AddTransient<IHL7MessageProcessor, HL7MessageProcessor>();
            services.AddTransient<IOpenHimOrchestrator, OpenHimOrchestrator>();

            services.AddOptions();
            services.Configure<HL7Config>(Configuration.GetSection("hl7Config"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("DockerDev"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenHim.Mediator.HL7Validator v1"));
            }

            // Setting up tls is a bit challenging...
            // It may not be needed due to the mediator ultimately not being publically exposed: Traffic will be coming
            // via the OpenHIM with ssl-offloading
            //app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
