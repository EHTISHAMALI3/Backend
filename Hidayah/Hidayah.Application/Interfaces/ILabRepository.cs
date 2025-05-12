using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Interfaces
{
    public interface ILabRepository
    {
        Task<mGeneric.mApiResponse<LabModel>> AddLabAsync(LabModel model);
        Task<mGeneric.mApiResponse<IEnumerable<LabModel>>> GetAllLabsAsync();
        Task<mGeneric.mApiResponse<LabModel>> GetLabByIdAsync(int labId);
        Task<mGeneric.mApiResponse<bool>> DeleteLabAsync(int labId);
        Task<mGeneric.mApiResponse<LabModel>> UpdateLabAsync(LabModel model);
    }
}
