using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Domain.Models.NoraniPrimer;
using Hidayah.Infrastrcture.AppDbContext;
using Hidayah.Infrastrcture.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hidayah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndividualLettersController : ControllerBase
    {
        private readonly IGenericRepository<IndividualLetters> _repo;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IndividualLettersController> _logger;

        public IndividualLettersController(
            IGenericRepository<IndividualLetters> repo,
            IWebHostEnvironment env,
            ILogger<IndividualLettersController> logger)
        {
            _repo = repo;
            _env = env;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllAsync();
            return StatusCode(result.RespCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repo.GetByIdAsync(id);
            return StatusCode(result.RespCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] IndividualLettersDto dto)
        {
            try
            {
                // Sanitize name: trim and remove all whitespace
                var cleanName = dto.Name?.Trim();
                cleanName = string.Concat(cleanName?.Where(c => !char.IsWhiteSpace(c)));

                // Check and save the SVG file
                string svgPath = null;
                if (dto.SvgFile != null)
                {
                    svgPath = await FileHelper.SaveFileAsync(dto.SvgFile, "assets/icons", cleanName, "svg");
                }

                // Check and save the Audio file
                string audioPath = null;
                if (dto.AudioFile != null)
                {
                    audioPath = await FileHelper.SaveFileAsync(dto.AudioFile, "assets/audio", cleanName, "audio");
                }

                // Create the entity and set its properties
                var entity = new IndividualLetters
                {
                    Name = cleanName,
                    Arabic = dto.Arabic,
                    Urdu = dto.Urdu,
                    SvgPath = svgPath,
                    AudioPath = audioPath,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                var result = await _repo.AddAsync(entity);
                return StatusCode(result.RespCode, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "File validation failed");
                return BadRequest(new mGeneric.mApiResponse<string>(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating IndividualLetter");
                return StatusCode(500, new mGeneric.mApiResponse<string>(500, "An error occurred while creating the record."));
            }
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _repo.GetAllPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] IndividualLettersDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing.RespData == null)
                return NotFound(new mGeneric.mApiResponse<string>(404, "Not found"));

            try
            {
                var entity = existing.RespData;

                // Sanitize name: trim and remove all whitespace
                var cleanName = dto.Name?.Trim();
                cleanName = string.Concat(cleanName?.Where(c => !char.IsWhiteSpace(c)));

                // Update other fields
                entity.Name = cleanName;
                entity.Arabic = dto.Arabic;
                entity.Urdu = dto.Urdu;
                entity.ModifiedBy = dto.CreatedBy;
                entity.ModifiedAt = DateTime.Now;

                // Validate and save SVG file if provided
                if (dto.SvgFile != null)
                {
                    try
                    {
                        entity.SvgPath = await FileHelper.SaveFileAsync(dto.SvgFile, "assets/icons", cleanName, "svg");
                    }
                    catch (InvalidOperationException ex)
                    {
                        return BadRequest(new mGeneric.mApiResponse<string>(400, ex.Message));
                    }
                }

                // Validate and save Audio file if provided
                if (dto.AudioFile != null)
                {
                    try
                    {
                        entity.AudioPath = await FileHelper.SaveFileAsync(dto.AudioFile, "assets/audio", cleanName, "audio");
                    }
                    catch (InvalidOperationException ex)
                    {
                        return BadRequest(new mGeneric.mApiResponse<string>(400, ex.Message));
                    }
                }

                var result = await _repo.UpdateAsync(entity);
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating IndividualLetter");
                return StatusCode(500, new mGeneric.mApiResponse<string>(500, "Update error"));
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.SoftDeleteAsync(id, "admin");
            return StatusCode(result.RespCode, result);
        }
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var result = await _repo.DeletePermanentAsync(id);
            return StatusCode(result.RespCode, result);
        }
    }
}