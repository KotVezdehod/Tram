using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Tram
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
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.Listen(IPAddress.Loopback, Args.port==0?5010:Args.port);
                options.Listen(IPAddress.Loopback, Args.ssl_port==0?5011:Args.ssl_port, (ListenOptions) =>
                {
                    if (Args.sert == "")
                    {
                        try
                        {
                            ListenOptions.UseHttps();
                        }
                        catch { }
                    }
                    else
                    {
                        if (Args.sert_pwd == "")
                        {
                            ListenOptions.UseHttps(Args.sert);
                        }
                        else
                        {
                            ListenOptions.UseHttps(Args.sert, Args.sert_pwd);
                        }
                    }
                });
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
