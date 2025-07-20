using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Square;
using Square.Webhooks;

namespace SquareUpMvc.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class SquareWebhookController : ControllerBase
    {
        private readonly string _signatureKey;

        public SquareWebhookController(IConfiguration configuration)
        {
            _signatureKey = configuration["Square:WebhookSignatureKey"];
        }

        [HttpPost("square-events")]
        public async Task<IActionResult> ReceiveSquareEvent()
        {
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["X-Square-Signature"];


            //if (!IsFromSquare(requestBody, signature))
            //{
            //    return Forbid("Signature validation failed.");
            //}

            try
            {
                var json = JObject.Parse(requestBody);
                var eventType = json["type"]?.ToString();

                Console.WriteLine($"--- Webhook Received: {eventType} ---");
                Console.WriteLine(requestBody);

                if (eventType == "payment.updated")
                {
                    var paymentStatus = json["data"]?["object"]?["payment"]?["status"]?.ToString();
                    if (paymentStatus == "COMPLETED")
                    {

                    }
                }


                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing webhook: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        private bool IsFromSquare(string requestBody, string signature)
        {
            var requestUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

            var notificationUrl = "https://hkdk.events/x1vqvpulhx8y8q";
            return WebhooksHelper.VerifySignature(
                requestBody: requestBody,
                signatureHeader: signature,
                signatureKey: _signatureKey,
                notificationUrl: notificationUrl
            );
        }
    }
}
