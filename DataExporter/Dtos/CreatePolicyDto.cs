using System.ComponentModel.DataAnnotations;

namespace DataExporter.Dtos
{
    public class CreatePolicyDto : IValidatableObject
    {
        [Required]
        public string PolicyNumber { get; set; }
        public decimal Premium { get; set; }
        public DateTime StartDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate == default(DateTime))
            {
                yield return new ValidationResult(
                    "Start date is required",
                    new[] { nameof(StartDate) }
                );
            }
            else if (StartDate.Date < DateTime.Today.Date)
            {
                yield return new ValidationResult(
                    "Start date cannot be in the past",
                    new[] { nameof(StartDate) }
                );
            }

            // This is assuming it is for new policies and not MTAs, which could produce negative premiums for refunds.
            if (Premium <= 0)
            {
                yield return new ValidationResult(
                    "Premium has to be greater than 0",
                    new[] { nameof(Premium) }
                );
            }
        }
    }
}
