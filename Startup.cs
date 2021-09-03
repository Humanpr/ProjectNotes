using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectNotes.Models;
using ProjectNotes.Services;

namespace ProjectNotes
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration,IWebHostEnvironment _env)
        {
            Configuration = configuration;
            env = _env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddCookiePolicy(policy=> {
                policy.Secure = CookieSecurePolicy.None;
            });

            services.AddApplicationInsightsTelemetry();
            services.AddControllersWithViews();

            if (!env.IsDevelopment())
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = (int) HttpStatusCode.PermanentRedirect;
                });
            }
            
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Access/Login";
            })

            #region OpenIdConnec and UserCheck

            .AddOpenIdConnect("Google",options=>{
                IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");

                string clientId = Configuration.GetValue<string>("GoogleClientId");
                string clientSecret = Configuration.GetValue<string>("GoogleClientSecret");

                options.Authority = "https://accounts.google.com";
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = "/signin-google";
                // After token is validated we check our DB records: If Db already contains record of user that signing in => pass.
                //                                                   If not: Then add new record for user.
                options.Events.OnTokenValidated = async context =>
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserServices>();
                    var checker = context.HttpContext.RequestServices.GetRequiredService<UserExistanceChecker>();
                    var nameIdentifier = context.Principal.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                    
                    if (await checker.CheckUserWithNameIdentifier(nameIdentifier))
                    {
                        // If user is in DB then redirect to Register or smth.
                        // context.HandleResponse();
                        // context.Response.Redirect("/Nope");
                    }
                    else
                    {
                        //Add user records to DB
                        Author author = new Author
                        {
                            AuthorName = context.Principal.Claims.SingleOrDefault(c=>c.Type == "name").Value,
                            NameIdentifier = nameIdentifier,  
                        };
                        await userService.AddUser(author);
                    }

                };
            });
            #endregion
            services.AddDbContext<NotesDbContext>(options=>{
                options.UseSqlServer(Configuration.GetValue<string>("SQLDBConnectionString"));
            });

            services.AddScoped<UserExistanceChecker>();
            services.AddScoped<UserServices>();
            services.AddScoped<IAuthorManager, SQLAuthorManager>();
            services.AddScoped<INoteManager, SQLNoteManager>();
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
                app.UseDeveloperExceptionPage();
                //TODO: Error page
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
