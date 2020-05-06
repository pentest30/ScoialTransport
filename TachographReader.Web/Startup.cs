using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using LazZiya.ExpressLocalization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tacchograaph_reader.Core.Entities;
using TachographReader.Application.Queries;
using TachographReader.Application.Services;
using TachographReader.Web.hubs;
using TachographReader.Web.LocalizationResources;
using TachographReader.Web.Migration;
using TachoReader.Data.Data;

namespace TachographReader.Web
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
            var cultures = new[]
            {
                new CultureInfo("fr"),
                new CultureInfo("en-US"),
                new CultureInfo("ar")

            };
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var assembly = Assembly.LoadFrom(string.Concat(path , Path.DirectorySeparatorChar +"TachographReader.Application.dll"));
            services.AddMediatR(assembly);
            services.AddRazorPages()
                .AddExpressLocalization<LocSource>(
                    ops =>
                    {
                        ops.ResourcesPath = "LocalizationResources";
                        ops.RequestLocalizationOptions = o =>
                        {
                            o.SupportedCultures = cultures;
                            o.SupportedUICultures = cultures;
                            o.DefaultRequestCulture = new RequestCulture("en-US");
                        };
                    });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<UserApp, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IDriverQueries, DriverQueries>();
            services.AddScoped<IDriverCarReportService, DriverCarReportService>();
            services.AddMediatR(typeof(Startup));
            services.AddSignalR();
            services
                .AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
           app.UseRequestLocalization();
            // app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=en-US}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<SocialDataHub>("/socialDataHub");
            });
            
            SeedInitialData(app);
            
        }
        private static void SeedInitialData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                #region seed identity data

                var identityContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var identitySeed = new SeedDummyData(identityContext);

                if (!identityContext.Roles.Any())
                {
                    identitySeed.CreateRolesAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                }
                identitySeed.CreateUsersAsync(scope.ServiceProvider).GetAwaiter().GetResult();
             

                #endregion
            }
        }
    }
}
