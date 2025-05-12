using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hidayah.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ILogger<BranchController> _logger;

        public BranchController(IBranchRepository branchRepository, ILogger<BranchController> logger)
        {
            _branchRepository = branchRepository;
            _logger = logger;
        }

        // ✅ Get All Branches
        [HttpGet]
        public async Task<ActionResult<mGeneric.mApiResponse<IEnumerable<BranchModel>>>> GetAll()
        {
            try
            {
                var result = await _branchRepository.GetAllAsync();
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                // Log error and return response
                _logger.LogError(ex, "Error occurred while fetching all branches.");
                return StatusCode(500, new mGeneric.mApiResponse<IEnumerable<BranchModel>>(500, "Server error occurred."));
            }
        }

        // ✅ Get Branch by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<BranchModel>>> GetById(string id)
        {
            try
            {
                var result = await _branchRepository.GetByIdAsync(id);
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                // Log error and return response
                _logger.LogError(ex, $"Error occurred while fetching branch with ID: {id}");
                return StatusCode(500, new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred."));
            }
        }

        // ✅ Add Branch
        [HttpPost]
        public async Task<ActionResult<mGeneric.mApiResponse<BranchModel>>> Add([FromBody] BranchModel model)
        {
            try
            {
                var result = await _branchRepository.AddAsync(model);
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                // Log error and return response
                _logger.LogError(ex, "Error occurred while adding a new branch.");
                return StatusCode(500, new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred."));
            }
        }

        // ✅ Update Branch
        [HttpPut("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<BranchModel>>> Update(string id, [FromBody] BranchModel model)
        {
            try
            {
                if (id != model.BranchId)
                {
                    return BadRequest(new mGeneric.mApiResponse<BranchModel>(400, "Mismatched Branch ID"));
                }

                var result = await _branchRepository.UpdateAsync(model);
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                // Log error and return response
                _logger.LogError(ex, $"Error occurred while updating branch with ID: {id}");
                return StatusCode(500, new mGeneric.mApiResponse<BranchModel>(500, "Server error occurred."));
            }
        }

        // ✅ Delete Branch
        [HttpDelete("{id}")]
        public async Task<ActionResult<mGeneric.mApiResponse<string>>> Delete(string id)
        {
            try
            {
                var result = await _branchRepository.DeleteAsync(id);
                return StatusCode(result.RespCode, result);
            }
            catch (Exception ex)
            {
                // Log error and return response
                _logger.LogError(ex, $"Error occurred while deleting branch with ID: {id}");
                return StatusCode(500, new mGeneric.mApiResponse<string>(500, "Server error occurred."));
            }
        }
    }
}
