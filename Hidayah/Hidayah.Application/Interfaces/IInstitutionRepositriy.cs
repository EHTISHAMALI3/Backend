using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Interfaces
{
    public interface IInstitutionRepositriy
    {
        Task<mGeneric.mApiResponse<IEnumerable<Institution>>> GetAllInstitute();
        Task<mGeneric.mApiResponse<Institution>> GetByIdInstitute(string id);
        Task<mGeneric.mApiResponse<Institution>> CreateInstitute(Institution institution);
        Task<mGeneric.mApiResponse<string>> UpdateInstitute(string id, Institution institution);
        Task<mGeneric.mApiResponse<string>> DeleteInstitute(string id);

    }
}
