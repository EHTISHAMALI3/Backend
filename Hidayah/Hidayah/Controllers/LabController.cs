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
    public class LabController : ControllerBase
    {
        private readonly ILabRepository _labRepository;
        private readonly ILogger<LabController> _logger;

        public LabController(ILabRepository labRepository, ILogger<LabController> logger)
        {
            _labRepository = labRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<mGeneric.mApiResponse<LabModel>>> AddLab([FromBody] LabModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new mGeneric.mApiResponse<LabModel>(400, "Invalid model"));

                var response = await _labRepository.AddLabAsync(model);
                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AddLab");
                return StatusCode(500, new mGeneric.mApiResponse<LabModel>(500, "Internal server error while adding lab."));
            }
        }

        [HttpGet]
        public async Task<ActionResult<mGeneric.mApiResponse<IEnumerable<LabModel>>>> GetAllLabs()
        {
            try
            {
                var response = await _labRepository.GetAllLabsAsync();
                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAllLabs");
                return StatusCode(500, new mGeneric.mApiResponse<IEnumerable<LabModel>>(500, "Internal server error while retrieving labs."));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<LabModel>>> GetLabById(int id)
        {
            try
            {
                var response = await _labRepository.GetLabByIdAsync(id);
                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GetLabById with id={id}");
                return StatusCode(500, new mGeneric.mApiResponse<LabModel>(500, "Internal server error while retrieving lab."));
            }
        }

        [HttpPut]
        public async Task<ActionResult<mGeneric.mApiResponse<LabModel>>> UpdateLab([FromBody] LabModel model)
        {
            try
            {
                var response = await _labRepository.UpdateLabAsync(model);
                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in UpdateLab with id={model?.LabId}");
                return StatusCode(500, new mGeneric.mApiResponse<LabModel>(500, "Internal server error while updating lab."));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<bool>>> DeleteLab(int id)
        {
            try
            {
                var response = await _labRepository.DeleteLabAsync(id);
                return StatusCode(response.RespCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in DeleteLab with id={id}");
                return StatusCode(500, new mGeneric.mApiResponse<bool>(500, "Internal server error while deleting lab."));
            }
        }
    }
}
