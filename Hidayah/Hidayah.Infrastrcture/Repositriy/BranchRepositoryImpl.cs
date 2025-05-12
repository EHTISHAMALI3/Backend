using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public class BranchRepositoryImpl : IBranchRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<BranchRepositoryImpl> _logger;
        public BranchRepositoryImpl(ApplicationDbContext applicationDbContext, ILogger<BranchRepositoryImpl> logger)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }
        public async Task<mGeneric.mApiResponse<BranchModel>> AddAsync(BranchModel model)
        {
            try
            {
                // Get the last branch ID
                var lastBranch = await _applicationDbContext.BGS_HA_TBL_BRANCHES
                    .OrderByDescending(b => b.BranchId)
                    .FirstOrDefaultAsync();

                int newIdNumber = 1;

                if (lastBranch != null && lastBranch.BranchId.StartsWith("BGS-BRN-"))
                {
                    // Extract the numeric part and increment
                    string lastIdNumber = lastBranch.BranchId.Replace("BGS-BRN-", "");
                    if (int.TryParse(lastIdNumber, out int lastNumber))
                    {
                        newIdNumber = lastNumber + 1;
                    }
                }

                // Format the new ID with leading zeros
                model.BranchId = $"BGS-BRN-{newIdNumber:D5}";
                model.CreatedAt = DateTime.Now;

                _applicationDbContext.BGS_HA_TBL_BRANCHES.Add(model);
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<BranchModel>(200, "Branch added successfully", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new branch");
                return new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred.");
            }
        }

        public async Task<mGeneric.mApiResponse<string>> DeleteAsync(string id)
        {
            try
            {
                var existing = await _applicationDbContext.BGS_HA_TBL_BRANCHES.FirstOrDefaultAsync(b => b.BranchId == id && !b.IsDeleted);
                if (existing == null)
                    return new mGeneric.mApiResponse<string>(404, "Branch not found");

                existing.IsDeleted = true;
                existing.ModifiedAt = DateTime.Now;
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Branch deleted successfully", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting branch: {id}");
                return new mGeneric.mApiResponse<string>(500, "Server error occurred.");
            }
        }

        public async Task<mGeneric.mApiResponse<IEnumerable<BranchModel>>> GetAllAsync()
        {
            try
            {
                var data = await _applicationDbContext.BGS_HA_TBL_BRANCHES.Where(b => !b.IsDeleted).ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<BranchModel>>(200, "Branches fetched successfully", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all branches");
                return new mGeneric.mApiResponse<IEnumerable<BranchModel>>(500, "Server error occurred.");
            }
        }

        public async Task<mGeneric.mApiResponse<BranchModel>> GetByIdAsync(string id)
        {
            try
            {
                var data = await _applicationDbContext.BGS_HA_TBL_BRANCHES.FirstOrDefaultAsync(b => b.BranchId == id && !b.IsDeleted);
                if (data == null)
                    return new mGeneric.mApiResponse<BranchModel>(404, "Branch not found");

                return new mGeneric.mApiResponse<BranchModel>(200, "Branch fetched", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching branch by ID: {id}");
                return new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred.");
            }
        }

        public async Task<mGeneric.mApiResponse<BranchModel>> UpdateAsync(BranchModel model)
        {
            try
            {
                var existing = await _applicationDbContext.BGS_HA_TBL_BRANCHES
                    .FirstOrDefaultAsync(b => b.BranchId == model.BranchId && !b.IsDeleted);

                if (existing == null)
                    return new mGeneric.mApiResponse<BranchModel>(404, "Branch not found");

                // ✅ Manual mapping
                existing.InstitutionId = model.InstitutionId;
                existing.BranchName = model.BranchName;
                existing.BranchCode = model.BranchCode;
                existing.BranchManagerId = model.BranchManagerId;
                existing.BranchEmail = model.BranchEmail;
                existing.BranchPhone = model.BranchPhone;
                existing.BranchAddress = model.BranchAddress;
                existing.Street = model.Street;
                existing.CityId = model.CityId;
                existing.State = model.State;
                existing.PostalCode = model.PostalCode;
                existing.CountryId = model.CountryId;
                existing.Status = model.Status;
                existing.IsDeleted = model.IsDeleted;

                // ✅ Audit info
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = model.ModifiedBy;

                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<BranchModel>(200, "Branch updated successfully", existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating branch: {model.BranchId}");
                return new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred.");
            }
        }

    }
}
