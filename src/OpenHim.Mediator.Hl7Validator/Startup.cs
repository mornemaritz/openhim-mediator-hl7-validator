using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenHim.Mediator.Hl7Validator.Extensions;

namespace OpenHim.Mediator.Hl7Validator
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenHim.Mediator.Hl7Validator", Version = "v1" });
            });

            // Due to current issues with xe-openhim.jembi.org:8082 returning a 401 Response due to
            // the token being expired (according to the openHIM logs), registration and heartbeats are currently disabled (in the deployed version).
            // Struggling to figure out what the issue is because the same authetication code works with a local containerised version of openHIM.
            services.AddOpenHimMediator(Configuration.GetSection("mediatorconfig"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("DockerDebug"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenHim.Mediator.Hl7Validator v1"));
            }

            // Setting up tls is a bit challenging...
            // It may not be needed due to the mediator ultimately not being publically exposed: Traffic will be coming
            // via the OpenHIM with ssl-offloading
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
