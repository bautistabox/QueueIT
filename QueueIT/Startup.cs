using System;
using System.Reflection;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QueueIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueIT.Identity;

namespace QueueIT
{
    public class Startup
    {
        private ILogger log;
        private IHostingEnvironment env;
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> log)
        {
            Configuration = configuration;
            this.log = log;
            this.env = env;

            RunMigrations();
        }

        private void RunMigrations()
        {
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(Configuration.GetConnectionString("QueueItDbConn"))
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                    (string s) => s.StartsWith("QueueIT.Migrations.Script"))
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

            if (upgradeEngine.IsUpgradeRequired())
            {
                log.LogInformation("Database Update Required.");

                var result = upgradeEngine.PerformUpgrade();

                if (!result.Successful)
                {
                    log.LogError(result.Error, "Failed to run migrations due to an error executing the sql scripts.");
                    throw result.Error;
                }
                
                log.LogInformation("Completed database upgrade. The following scripts were executed");
                foreach (var script in result.Scripts)
                {
                    log.LogInformation(script.Name);
                }
            }
            else
            {
                log.LogInformation("No database update is required.");
            }
        }
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<QueueItUserDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("QueueItUserDbConn"),
                sql => sql.MigrationsAssembly(migrationAssembly)));
            services.AddDbContext<QueueItDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("QueueItDbConn")));
            services.AddIdentity<QueueItUser, IdentityRole>(options =>
                    {
                        
                        options.Tokens.EmailConfirmationTokenProvider = "emailconf";
                        
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequiredUniqueChars = 4;

                        options.User.RequireUniqueEmail = true;
                    })
                .AddEntityFrameworkStores<QueueItUserDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<QueueItUser>>("emailconf")
                .AddPasswordValidator<DoesNotContainPasswordValidator<QueueItUser>>();
            
            services.AddScoped<IUserClaimsPrincipalFactory<QueueItUser>, QueueItUserClaimsPrincipalFactory>();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromDays(2));
            services.Configure<PasswordHasherOptions>(options => { options.IterationCount = 100000; });
            
            services.ConfigureApplicationCookie(options => options.LoginPath = "/home/index");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}