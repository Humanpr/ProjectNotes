using System;
using System.Collections.Generic;
using System.Linq;
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
        

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCookiePolicy(policy=> {
                policy.Secure = CookieSecurePolicy.Always;
            });

            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Access/Login";
            })
            .AddOpenIdConnect("Google",options=>{
                IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");
                options.Authority = "https://accounts.google.com";
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
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

            services.AddDbContext<NotesDbContext>(options=>{
                options.UseSqlServer("server = localhost; database = NotesDB; Trusted_Connection = true ");
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
                //TODO: Error page
                app.UseExceptionHandler("/Home/Error");
            }
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
