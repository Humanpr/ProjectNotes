using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectNotes.Infrastructure.Persistance;
using MediatR;
using ProjectNotes.Core.Interfaces;
using ProjectNotes.Infrastructure.Services;

namespace ProjectNotes.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotesDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetValue<string>("SQLDBConnectionString"));
            });

            services.AddScoped<NoteTweeter>();
            services.AddScoped<IAuthorManager, SQLAuthorManager>();
            services.AddScoped<INoteManager, SQLNoteManager>();
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
