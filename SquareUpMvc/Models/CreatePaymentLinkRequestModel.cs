namespace SquareUpMvc.Models
{
    public class CreatePaymentLinkRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
    }
} 