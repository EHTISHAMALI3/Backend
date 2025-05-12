using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Interfaces
{
    public interface IInstitutionTypeRepository
    {
        Task<mGeneric.mApiResponse<InstitutionType>> CreateInstitutionType(InstitutionType institutionType);
        Task<mGeneric.mApiResponse<InstitutionType>> GetInstitutionTypeById(string id);
        Task<mGeneric.mApiResponse<IEnumerable<InstitutionType>>> GetAllInstitutionTypes();
        Task<mGeneric.mApiResponse<IEnumerable<Country>>> GetAllCountry();
        Task<mGeneric.mApiResponse<IEnumerable<City>>> GetAllCity();
        Task<mGeneric.mApiResponse<bool>> UpdateInstitutionType(InstitutionType institutionType);
        Task<mGeneric.mApiResponse<bool>> DeleteInstitutionType(string id);
    }
}
