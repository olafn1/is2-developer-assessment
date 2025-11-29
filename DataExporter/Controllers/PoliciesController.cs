using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private PolicyService _policyService;

        public PoliciesController(PolicyService policyService) 
        { 
            _policyService = policyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("Policies API");
        }

        [HttpPost("PostPolicies")]
        public async Task<IActionResult> PostPolicies([FromBody]CreatePolicyDto createPolicyDto)
        {
            var newPolicy = await _policyService.CreatePolicyAsync(createPolicyDto);

            if (newPolicy == null)
            {
                return NotFound(new { Message = $"Policy could not have been created." });
            }

            return Ok(newPolicy);
        }

        [HttpGet("GetPolicies")]
        public async Task<IActionResult> GetPolicies()
        {
            var policies = await _policyService.ReadPoliciesAsync();

            if (policies == null)
            {
                return NotFound(new { Message = $"No policies have been found." });
            }

            return Ok(policies);
        }

        [HttpGet("GetPolicy/{id}")]
        public async Task<IActionResult> GetPolicy(int id)
        {
            // Possible improvements, could add cancellation token to the endpoints, however for getting a policy by id it shouldn't be necessary
            // Additionally could create standard api response class/object for all enpoints to use, but it is not stricly necessary either

            var policy = await _policyService.ReadPolicyAsync(id);

            if (policy == null) 
            {
                return NotFound(new { Message = $"Policy with ID {id} was not found." });
            }

            return Ok(policy);
        }

        [HttpGet("Export")]
        public async Task<IActionResult> ExportData([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
        {
            var policies = await _policyService.ExportPoliciesAsync(startDate, endDate);

            if (policies == null || policies.Count == 0)
            {
                return NotFound(new { Message = $"Policies with start date between {startDate} and {endDate} not found." });
            }

            return Ok(policies);
        }
    }
}
