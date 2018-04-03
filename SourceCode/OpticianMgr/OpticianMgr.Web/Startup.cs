using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpticianMgr.WebIdentity.Data;
using OpticianMgr.WebIdentity.Models;
using OpticianMgr.WebIdentity.Services;
using Microsoft.AspNetCore.Identity;
using OpticiatnMgr.Core.Contracts;
using OpticianMgr.Persistence;

namespace OpticianMgr.WebIdentity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
          //      builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultIdentityConnection")));

            services.AddTransient<IUnitOfWork, UnitOfWork>(p => new UnitOfWork(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Data.ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
                
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        public async void InitRoles(IServiceProvider provider)
        {
            var RoleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            await CreateRole(RoleManager, "Admin");

            await UserManager.CreateAsync(new ApplicationUser { UserName = "orascanin.99@gmail.com" });

            await AddUserRole("orascanin.99@gmail.com", "Admin", UserManager, RoleManager);

        }

        private async Task CreateRole(RoleManager<IdentityRole> rm, string roleName)
        {
            IdentityResult roleResult = null;

            if (!await rm.RoleExistsAsync(roleName))
            {
                roleResult = await rm.CreateAsync(new IdentityRole(roleName));
            }
            if (roleResult == null || !roleResult.Succeeded)
            {
                Console.WriteLine("Fehler: Rolle " + roleName + " konnte nicht erstellt werden!");
            }
        }
        private async Task AddUserRole(string name, string roleName, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            var user = await um.FindByNameAsync(name);
            var role = await rm.FindByNameAsync(roleName);
            var inRole = await um.IsInRoleAsync(user, roleName);
            IdentityResult result = null;

            if (user != null && !inRole)
            {
                result = await um.AddToRoleAsync(user, roleName);
            }
            if (result == null || !result.Succeeded)
            {
                Console.WriteLine("Fehler: User " + name + "konnte nicht zur Rolle " + roleName + " hinzugefügt werden!");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            InitRoles(provider);
        }
    }
}
