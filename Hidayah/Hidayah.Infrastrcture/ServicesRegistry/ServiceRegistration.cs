using Hidayah.Application.Interfaces;
using Hidayah.Infrastrcture.Repositriy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.ServicesRegistry
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection builder)
        {
            builder.AddScoped<IUserRepositriy, UserRepositoryImpl>();
            builder.AddScoped<IInstitutionTypeRepository, InstitutionTypeRepositriyImpl>();
            builder.AddScoped<IInstitutionRepositriy, InstitutionRepositriyImpl>();
            builder.AddScoped<IBranchRepository, BranchRepositoryImpl>();
            builder.AddScoped<ILabRepository, LabRepositoryimpl>();
            builder.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepositoryImpl<>));

        }
    }
}
