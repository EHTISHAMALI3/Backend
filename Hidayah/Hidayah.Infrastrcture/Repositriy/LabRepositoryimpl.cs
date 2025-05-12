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
    public class LabRepositoryimpl : ILabRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LabRepositoryimpl> _logger;

        public LabRepositoryimpl(ApplicationDbContext context, ILogger<LabRepositoryimpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<mGeneric.mApiResponse<LabModel>> AddLabAsync(LabModel model)
        {
            try
            {
                model.CreatedAt = DateTime.Now;
                _context.BGS_HA_TBL_LABS.Add(model);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<LabModel>(200, "Lab added successfully", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding lab");
                return new mGeneric.mApiResponse<LabModel>(500, "An error occurred while adding the lab.");
            }
        }

        public async Task<mGeneric.mApiResponse<IEnumerable<LabModel>>> GetAllLabsAsync()
        {
            try
            {
                var labs = await _context.BGS_HA_TBL_LABS.Where(x => !x.IsDeleted).ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<LabModel>>(200, "Labs retrieved", labs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving labs");
                return new mGeneric.mApiResponse<IEnumerable<LabModel>>(500, "Failed to retrieve labs.");
            }
        }

        public async Task<mGeneric.mApiResponse<LabModel>> GetLabByIdAsync(int labId)
        {
            try
            {
                var lab = await _context.BGS_HA_TBL_LABS.FindAsync(labId);
                if (lab == null || lab.IsDeleted)
                    return new mGeneric.mApiResponse<LabModel>(404, "Lab not found");

                return new mGeneric.mApiResponse<LabModel>(200, "Lab retrieved", lab);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lab by ID");
                return new mGeneric.mApiResponse<LabModel>(500, "Error occurred while fetching the lab.");
            }
        }

        public async Task<mGeneric.mApiResponse<LabModel>> UpdateLabAsync(LabModel model)
        {
            try
            {
                var existingLab = await _context.BGS_HA_TBL_LABS.FindAsync(model.LabId);
                if (existingLab == null || existingLab.IsDeleted)
                    return new mGeneric.mApiResponse<LabModel>(404, "Lab not found");

                _context.Entry(existingLab).CurrentValues.SetValues(model);
                existingLab.ModifiedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<LabModel>(200, "Lab updated successfully", existingLab);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lab");
                return new mGeneric.mApiResponse<LabModel>(500, "Error occurred while updating the lab.");
            }
        }

        public async Task<mGeneric.mApiResponse<bool>> DeleteLabAsync(int labId)
        {
            try
            {
                var lab = await _context.BGS_HA_TBL_LABS.FindAsync(labId);
                if (lab == null || lab.IsDeleted)
                    return new mGeneric.mApiResponse<bool>(404, "Lab not found");

                lab.IsDeleted = true;
                lab.ModifiedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<bool>(200, "Lab deleted", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lab");
                return new mGeneric.mApiResponse<bool>(500, "Failed to delete lab.");
            }
        }
    }
}
