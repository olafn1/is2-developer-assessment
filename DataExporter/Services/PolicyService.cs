using DataExporter.Dtos;
using Microsoft.EntityFrameworkCore;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
        {
            var newPolicy = new Model.Policy()
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };

            await _dbContext.Policies.AddAsync(newPolicy);
            await _dbContext.SaveChangesAsync();

            return new ReadPolicyDto()
            {
                Id = newPolicy.Id,
                PolicyNumber = newPolicy.PolicyNumber,
                Premium = newPolicy.Premium,
                StartDate = newPolicy.StartDate
            };
        }

        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a list of ReadPoliciesDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
        {
            var policies = await _dbContext.Policies.Select(x => new ReadPolicyDto
            {
                Id = x.Id,
                PolicyNumber = x.PolicyNumber,
                Premium = x.Premium,
                StartDate = x.StartDate
            }).ToListAsync();

            return policies;
            
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ReadPolicyDto.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id)
        {
            // Possible improvements - we could use linq for the whole statement below
            // Assuming ids are primary keys and there won't be duplicates we could use FindAsync or FirstOrDefaultAsync instead
            
            var policy = await _dbContext.Policies.SingleOrDefaultAsync(x => x.Id == id);
            if (policy == null)
            {
                return null;
            }

            var policyDto = new ReadPolicyDto()
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };

            return policyDto;
        }

        /// <summary>
        /// Retrieves all policies with their notes between specific dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns a list of ExportDto objects.</returns>
        public async Task<IList<ExportDto>> ExportPoliciesAsync(DateTime startDate, DateTime endDate)
        {

            var policies = await _dbContext.Policies.Where(x => x.StartDate >= startDate && x.StartDate <= endDate).Select(x => new ExportDto
            {
                PolicyNumber = x.PolicyNumber,
                Premium = x.Premium,
                StartDate = x.StartDate,
                Notes = x.Notes.Select(n => n.Text).ToList()
            }).ToListAsync();

            return policies;
        }

    }
}
