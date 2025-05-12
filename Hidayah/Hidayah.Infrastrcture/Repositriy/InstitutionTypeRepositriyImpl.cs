using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public class InstitutionTypeRepositriyImpl : IInstitutionTypeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstitutionTypeRepositriyImpl> _logger;
        public InstitutionTypeRepositriyImpl(ApplicationDbContext context, ILogger<InstitutionTypeRepositriyImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<mGeneric.mApiResponse<InstitutionType>> CreateInstitutionType(InstitutionType institutionType)
        {
            try
            {
                await _context.BGS_HA_TBL_INSTITUTION_TYPES.AddAsync(institutionType);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<InstitutionType>(200, "Institution Type created successfully.", institutionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating institution type.");
                return new mGeneric.mApiResponse<InstitutionType>(500, "Internal server error while creating institution type.");
            }
        }

        public async Task<mGeneric.mApiResponse<bool>> DeleteInstitutionType(string id)
        {
            try
            {
                var institutionType = await _context.BGS_HA_TBL_INSTITUTION_TYPES.FindAsync(id);
                if (institutionType == null)
                {
                    return new mGeneric.mApiResponse<bool>(404, "Institution Type not found.");
                }

                _context.BGS_HA_TBL_INSTITUTION_TYPES.Remove(institutionType);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<bool>(200, "Institution Type deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting institution type.");
                return new mGeneric.mApiResponse<bool>(500, "Internal server error while deleting institution type.");
            }
        }

        public async Task<mGeneric.mApiResponse<IEnumerable<InstitutionType>>> GetAllInstitutionTypes()
        {
            try
            {
                var result = await _context.BGS_HA_TBL_INSTITUTION_TYPES.ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<InstitutionType>>(200, "Institution Types retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching institution types.");
                return new mGeneric.mApiResponse<IEnumerable<InstitutionType>>(500, "Internal server error while fetching institution types.");
            }
        }
        public async Task<mGeneric.mApiResponse<IEnumerable<Country>>> GetAllCountry()
        {
            try
            {
                var result = await _context.BGS_HA_TBL_COUNTRY.ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<Country>>(200, "Countries retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching institution types.");
                return new mGeneric.mApiResponse<IEnumerable<Country>>(500, "Internal server error while fetching countries.");
            }
        }
        public async Task<mGeneric.mApiResponse<IEnumerable<City>>> GetAllCity()
        {
            try
            {
                var result = await _context.BGS_HA_TBL_CITY.ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<City>>(200, "Countries retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching institution types.");
                return new mGeneric.mApiResponse<IEnumerable<City>>(500, "Internal server error while fetching countries.");
            }
        }
        public async Task<mGeneric.mApiResponse<InstitutionType>> GetInstitutionTypeById(string id)
        {
            try
            {
                var result = await _context.BGS_HA_TBL_INSTITUTION_TYPES
                    .SingleOrDefaultAsync(it => it.InstitutionTypeId == id);

                if (result == null)
                {
                    return new mGeneric.mApiResponse<InstitutionType>(404, "Institution Type not found.");
                }

                return new mGeneric.mApiResponse<InstitutionType>(200, "Institution Type retrieved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching institution type.");
                return new mGeneric.mApiResponse<InstitutionType>(500, "Internal server error while fetching institution type.");
            }
        }

        public async Task<mGeneric.mApiResponse<bool>> UpdateInstitutionType(InstitutionType institutionType)
        {
            try
            {
                _context.BGS_HA_TBL_INSTITUTION_TYPES.Update(institutionType);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<bool>(200, "Institution Type updated successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating institution type.");
                return new mGeneric.mApiResponse<bool>(500, "Internal server error while updating institution type.");
            }
        }
    }
}
