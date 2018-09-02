using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetFeatureFolders
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
            services.AddMvc()
                .AddRazorOptions(razorOptions =>
                {
                    // Standard feature views
                    razorOptions.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
                    razorOptions.ViewLocationFormats.Add("/Features/{0}.cshtml");
                    razorOptions.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");

                    // Area feature views
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/Features/{1}/{0}.cshtml");
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/Features/{0}.cshtml");
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/Features/Shared/{0}.cshtml");

                    // Feature folder area views
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/{1}/{0}.cshtml");
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/{0}.cshtml");
                    razorOptions.AreaViewLocationFormats.Add("/Areas/{2}/Shared/{0}.cshtml");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
