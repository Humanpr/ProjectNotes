using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectNotes.Models;

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

        public void ConfigureServices(IServiceCollection services)
        {


            services.AddCookiePolicy(policy=> {
                policy.Secure = CookieSecurePolicy.Always;
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
            });
            services.AddDbContext<NotesDbContext>(options=>{
                options.UseSqlServer(Configuration.GetValue<string>("SQLDBConnectionString"));
            });

            services.AddScoped<NoteTweeter>();

            services.AddScoped<IAuthorManager, SQLAuthorManager>();
            services.AddScoped<INoteManager, SQLNoteManager>();

            services.AddMediatR(typeof(AddUserCommand).Assembly);
        }

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
