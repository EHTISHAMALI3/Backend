using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<mGeneric.mApiResponse<List<T>>> GetAllAsync();
        Task<mGeneric.mApiResponse<T>> GetByIdAsync(int id);
        Task<mGeneric.mApiResponse<string>> AddAsync(T entity);
        Task<mGeneric.mApiResponse<string>> UpdateAsync(T entity);
        Task<mGeneric.mApiResponse<string>> SoftDeleteAsync(int id, string modifiedBy);
        Task<mGeneric.mApiResponse<string>> DeletePermanentAsync(int id);
        Task<mGeneric.mApiResponse<PaginatedResponseDto<T>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
    }
}
