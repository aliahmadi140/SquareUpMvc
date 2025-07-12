using System.ComponentModel.DataAnnotations;

namespace SquareUpMvc.Models
{
    public class PaymentRequestModel
    {
        [Required(ErrorMessage = "Source ID is required")]
        public string SourceId { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public long Amount { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters")]
        public string Currency { get; set; } = "GBP";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
    }
}
