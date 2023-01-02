using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Retail_Banking.Data;
using Retail_Banking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Retail_Banking
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
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default Connection")));
            services.AddDbContext<ManagementContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default Connection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddTransient<CustomerInterface, CustomerRepo>();
            services.AddTransient<ErrorInterface, ErrorRepo>();
            services.AddTransient<AccountInterface, AccountRepo>();
            services.AddTransient<TransactionsInterface, TransactionsRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Middleware");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //context.Database.EnsureDeleted(); //Add to clear db at startup.
                context.Database.Migrate();
            }

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            CreateRoles(serviceProvider).Wait();

            app.Use(async (context, next) =>
            {
                var myTimer = System.Diagnostics.Stopwatch.StartNew();
                logger.LogInformation($"Beginning request in {env.EnvironmentName} environment");

                await next();
                logger.LogInformation($"Request completed in {myTimer.ElapsedMilliseconds}ms");
            });

            app.Run((context) => {
                //context.Response.ContentType = "text/html";
                //await context.Response.WriteAsync("Message displayed by middleware!");
                context.Response.Redirect("https://localhost:5001/error/error");
                return Task.CompletedTask;
            });
        }

        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "UserManager", "Manager", "Worker", "User" };

            IdentityResult identityResult;

            foreach(var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    identityResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var usermanager = await UserManager.FindByEmailAsync("UserManager@UserManager.com");
            if (usermanager == null)
            {
                var userManager = new IdentityUser { UserName = "UserManager@UserManager.com", Email = "UserManager@UserManager.com", EmailConfirmed = true };
                var createUserManager = await UserManager.CreateAsync(userManager,"UserManager2022!");
                if (createUserManager.Succeeded)
                {
                    await UserManager.AddToRoleAsync(userManager, "UserManager");
                }
            }
        }
    }
}
