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
    public class InstitutionRepositriyImpl : IInstitutionRepositriy
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstitutionRepositriyImpl> _logger;
        public InstitutionRepositriyImpl(ApplicationDbContext context, ILogger<InstitutionRepositriyImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<mGeneric.mApiResponse<Institution>> CreateInstitute(Institution institution)
        {
            try
            {
                // Get the last institution ID
                var lastInstitution = await _context.BGS_HA_TBL_INSTITUTION
                    .Where(i => i.InstitutionId.StartsWith("BGS-INS-"))
                    .OrderByDescending(i => i.InstitutionId)
                    .FirstOrDefaultAsync();

                int newIdNumber = 1000; // Starting from 1000 as requested

                if (lastInstitution != null)
                {
                    // Extract the number part and increment it
                    string lastId = lastInstitution.InstitutionId.Replace("BGS-INS-", "");
                    if (int.TryParse(lastId, out int lastNumber) && lastNumber >= 1000)
                    {
                        newIdNumber = lastNumber + 1;
                    }
                }

                // Format new ID with leading zeros (5 digits total)
                institution.InstitutionId = $"BGS-INS-{newIdNumber:D5}";
                institution.CreatedAt = DateTime.Now;

                _context.BGS_HA_TBL_INSTITUTION.Add(institution);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<Institution>(200, "Institution created", institution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync");
                return new mGeneric.mApiResponse<Institution>(500, "Failed to create institution");
            }
        }

        public async Task<mGeneric.mApiResponse<IEnumerable<Institution>>> GetAllInstitute()
        {
            try
            {
                var list = await _context.BGS_HA_TBL_INSTITUTION.AsNoTracking().ToListAsync();
                return new mGeneric.mApiResponse<IEnumerable<Institution>>(200, "Fetched successfully", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync");
                return new mGeneric.mApiResponse<IEnumerable<Institution>>(500, "Internal Server Error");
            }
        }

        public async Task<mGeneric.mApiResponse<Institution>> GetByIdInstitute(string id)
        {
            try
            {
                var item = await _context.BGS_HA_TBL_INSTITUTION.FindAsync(id);
                if (item == null)
                    return new mGeneric.mApiResponse<Institution>(404, "Institution not found");

                return new mGeneric.mApiResponse<Institution>(200, "Success", item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync({id})");
                return new mGeneric.mApiResponse<Institution>(500, "Internal Server Error");
            }
        }

        public async Task<mGeneric.mApiResponse<string>> DeleteInstitute(string id)
        {
            try
            {
                var institution = await _context.BGS_HA_TBL_INSTITUTION.FindAsync(id);
                if (institution == null)
                    return new mGeneric.mApiResponse<string>(404, "Institution not found");

                institution.IsDeleted = true;
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Institution soft-deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SoftDeleteAsync({id})");
                return new mGeneric.mApiResponse<string>(500, "Failed to delete institution");
            }
        }

        public async Task<mGeneric.mApiResponse<string>> UpdateInstitute(string id, Institution institution)
        {
            try
            {
                var existing = await _context.BGS_HA_TBL_INSTITUTION.FindAsync(id);
                if (existing == null)
                    return new mGeneric.mApiResponse<string>(404, "Institution not found");

                institution.InstitutionId = id;
                //existing.InstitutionId = institution.InstitutionId;
                existing.InstitutionName = institution.InstitutionName;
                existing.InstitutionEmail = institution.InstitutionEmail;
                existing.InstitutionPhone = institution.InstitutionPhone;
                existing.WebsiteUrl = institution.WebsiteUrl;
                existing.DateOfEstablishment = institution.DateOfEstablishment;
                existing.CountryId = institution.CountryId;
                existing.CityId = institution.CityId;
                existing.InstitutionTypeId = institution.InstitutionTypeId;
                existing.State = institution.State;
                existing.PostalCode = institution.PostalCode;
                existing.AddressLine = institution.AddressLine;
                existing.PrimaryContactPerson = institution.PrimaryContactPerson;
                existing.PrimaryContactFullName = institution.PrimaryContactFullName;
                existing.PrimaryContactPhoneNumber = institution.PrimaryContactPhoneNumber;
                existing.PrimaryContactEmail = institution.PrimaryContactEmail;
                existing.PrimaryContactJobTitle = institution.PrimaryContactJobTitle;
                existing.InstitutionStatus = institution.InstitutionStatus;
                existing.CreatedBy = institution.CreatedBy;
                existing.CreatedAt = institution.CreatedAt;
                existing.ModifiedBy = institution.ModifiedBy;
                institution.ModifiedAt = DateTime.Now;
                _context.Entry(existing).CurrentValues.SetValues(institution);
                await _context.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Institution updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateAsync({id})");
                return new mGeneric.mApiResponse<string>(500, "Failed to update institution");
            }
        }
    }
}
