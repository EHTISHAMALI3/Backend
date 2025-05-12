using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hidayah.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionRepositriy _institutionRepository;
        private readonly ILogger<InstitutionController> _logger;
        public InstitutionController(IInstitutionRepositriy institutionRepository, ILogger<InstitutionController> logger)
        {
            _institutionRepository = institutionRepository;
            _logger = logger;
        }
        // POST: api/Institution
        [HttpPost]
        public async Task<ActionResult<mGeneric.mApiResponse<Institution>>> CreateInstitute([FromBody] Institution institution)
        {
            try
            {
                if (institution == null)
                {
                    _logger.LogWarning("CreateInstitute: Institution data is null");
                    return BadRequest(new mGeneric.mApiResponse<Institution>(400, "Invalid institution data"));
                }

                var response = await _institutionRepository.CreateInstitute(institution);
                if (response.RespCode == 200)
                    return CreatedAtAction(nameof(GetByIdInstitute), new { id = institution.InstitutionId }, response);

                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateInstitute");
                return StatusCode(500, new mGeneric.mApiResponse<Institution>(500, "An error occurred while creating the institution"));
            }
        }

        // GET: api/Institution
        [HttpGet]
        public async Task<ActionResult<mGeneric.mApiResponse<IEnumerable<Institution>>>> GetAllInstitutes()
        {
            try
            {
                var response = await _institutionRepository.GetAllInstitute();
                if (response.RespCode == 200)
                    return Ok(response);

                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllInstitutes");
                return StatusCode(500, new mGeneric.mApiResponse<IEnumerable<Institution>>(500, "An error occurred while fetching institutions"));
            }
        }

        // GET: api/Institution/5
        [HttpGet("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<Institution>>> GetByIdInstitute(string id)
        {
            try
            {
                var response = await _institutionRepository.GetByIdInstitute(id);
                if (response.RespCode == 200)
                    return Ok(response);

                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdInstitute with id {id}");
                return StatusCode(500, new mGeneric.mApiResponse<Institution>(500, "An error occurred while fetching the institution"));
            }
        }

        // DELETE: api/Institution/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<string>>> DeleteInstitute(string id)
        {
            try
            {
                var response = await _institutionRepository.DeleteInstitute(id);
                if (response.RespCode == 200)
                    return Ok(response);

                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteInstitute with id {id}");
                return StatusCode(500, new mGeneric.mApiResponse<string>(500, "An error occurred while deleting the institution"));
            }
        }

        // PUT: api/Institution/5
        [HttpPut("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<string>>> UpdateInstitute(string id, [FromBody] Institution institution)
        {
            try
            {
                if (institution == null)
                {
                    _logger.LogWarning("UpdateInstitute: Institution data is null");
                    return BadRequest(new mGeneric.mApiResponse<string>(400, "Invalid institution data"));
                }

                var response = await _institutionRepository.UpdateInstitute(id, institution);
                if (response.RespCode == 200)
                    return Ok(response);

                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateInstitute with id {id}");
                return StatusCode(500, new mGeneric.mApiResponse<string>(500, "An error occurred while updating the institution"));
            }
        }
    }
}
