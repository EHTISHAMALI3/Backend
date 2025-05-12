using Hidayah.Application.Generic;
using Hidayah.Domain.Models.NoraniPrimer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public interface IIndividualLetterRepository
    {
        Task<mGeneric.mApiResponse<List<IndividualLetters>>> GetAllAsync();
        Task<mGeneric.mApiResponse<IndividualLetters>> GetByIdAsync(int id);
        Task<mGeneric.mApiResponse<string>> CreateAsync(IndividualLetters model);
        Task<mGeneric.mApiResponse<string>> UpdateAsync(IndividualLetters model);
        Task<mGeneric.mApiResponse<string>> SoftDeleteAsync(int id, string modifiedBy);
    }
}
