using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Interfaces
{
    public interface IBranchRepository
    {
        Task<mGeneric.mApiResponse<IEnumerable<BranchModel>>> GetAllAsync();
        Task<mGeneric.mApiResponse<BranchModel>> GetByIdAsync(string id);
        Task<mGeneric.mApiResponse<BranchModel>> AddAsync(BranchModel model);
        Task<mGeneric.mApiResponse<BranchModel>> UpdateAsync(BranchModel model);
        Task<mGeneric.mApiResponse<string>> DeleteAsync(string id);
    }
}
