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
    public class InstitutionTypeController : ControllerBase
    {
        private readonly IInstitutionTypeRepository _institutionTypeRepository;

        public InstitutionTypeController(IInstitutionTypeRepository institutionTypeRepository)
        {
            _institutionTypeRepository = institutionTypeRepository;
        }

        // Create a new institution type
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                var response = await _institutionTypeRepository.CreateInstitutionType(institutionType);
                if (response.RespCode == 200)
                {
                    return Ok(response);
                }
                return StatusCode(response.RespCode, response); // Return appropriate status code and message
            }
            else
            {
                return BadRequest(new mGeneric.mApiResponse<string>(400, "Invalid data."));
            }
        }

        // Get all institution types
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _institutionTypeRepository.GetAllInstitutionTypes();
            if (response.RespCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.RespCode, response);
        }

        // Get all institution country
        [HttpGet("/api/getCountry")]
        public async Task<IActionResult> GetAllCountry()
        {
            var response = await _institutionTypeRepository.GetAllCountry();
            if (response.RespCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.RespCode, response);
        }
        // Get all institution city
        [HttpGet("/api/getCity")]
        public async Task<IActionResult> GetAllCity()
        {
            var response = await _institutionTypeRepository.GetAllCity();
            if (response.RespCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.RespCode, response);
        }
        // Get institution type by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _institutionTypeRepository.GetInstitutionTypeById(id);
            if (response.RespCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.RespCode, response);
        }

        // Update an institution type
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                var response = await _institutionTypeRepository.UpdateInstitutionType(institutionType);
                if (response.RespCode == 200)
                {
                    return Ok(response);
                }
                return StatusCode(response.RespCode, response);
            }
            return BadRequest(new mGeneric.mApiResponse<string>(400, "Invalid data."));
        }

        // Delete institution type by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _institutionTypeRepository.DeleteInstitutionType(id);
            if (response.RespCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.RespCode, response);
        }
    }
}