﻿
using Microsoft.AspNetCore.Mvc;
using Square;
using Square.Checkout.PaymentLinks;
using Square.Customers;
using Square.Payments;
using SquareUpMvc.Models;
using Square.Checkout;

namespace SquareUpMvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly SquareClient _squareClient;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(SquareClient squareClient, ILogger<PaymentController> logger)
        {
            _squareClient = squareClient;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestModel request)
        {
            try
            {
                // Validate input
                if (request == null)
                {
                    return BadRequest(new { Status = "Failed", Message = "Payment request is required" });
                }

                if (string.IsNullOrEmpty(request.SourceId))
                {
                    return BadRequest(new { Status = "Failed", Message = "Source ID is required" });
                }

                if (request.Amount <= 0)
                {
                    return BadRequest(new { Status = "Failed", Message = "Amount must be greater than 0" });
                }

                if (string.IsNullOrEmpty(request.Currency))
                {
                    return BadRequest(new { Status = "Failed", Message = "Currency is required" });
                }

                _logger.LogInformation("Processing payment: Amount={Amount}, Currency={Currency}",
                    request.Amount, request.Currency);


                var amountMoney = new Money
                {
                    Amount = request.Amount,
                    Currency = new Currency(request.Currency.ToUpper())
                };



                var currentCustomer = await _squareClient.Customers.SearchAsync(
            new SearchCustomersRequest
            {
                Query = new CustomerQuery
                {
                    Filter = new CustomerFilter
                    {
                        EmailAddress = new CustomerTextFilter
                        {
                            Fuzzy = "ali123@gmail.com"
                        }
                    }
                }
            });

                CreateCustomerResponse newCustomer = new();
                if (currentCustomer.Customers == null)
                {
                    newCustomer = await _squareClient.Customers.CreateAsync(
     new CreateCustomerRequest
     {
         GivenName = "ali",
         FamilyName = "Earhart",
         EmailAddress = "ali123@gmail.com",

     }
 );
                }

                var customerId = currentCustomer.Customers != null ? currentCustomer.Customers.FirstOrDefault().Id : newCustomer.Customer.Id;


                var createPaymentRequest = new CreatePaymentRequest
                {
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    SourceId = request.SourceId,
                    AmountMoney = amountMoney,
                    CustomerId = customerId,
                    ReferenceId = "aliali"
                };

                var response = await _squareClient.Payments.CreateAsync(createPaymentRequest);


                if (response.Payment.Status == "COMPLETED")
                {
                    _logger.LogInformation("Payment completed successfully: PaymentId={PaymentId}",
                        response.Payment.Id);



                    return Ok(new
                    {
                        Status = "Success",
                        PaymentId = response.Payment.Id,
                        Amount = response.Payment.AmountMoney.Amount,
                        Currency = response.Payment.AmountMoney.Currency.ToString()
                    });
                }
                else
                {
                    _logger.LogWarning("Payment not completed: Status={Status}, PaymentId={PaymentId}",
                        response.Payment.Status, response.Payment.Id);

                    return BadRequest(new
                    {
                        Status = "Failed",
                        Message = $"Payment was not completed. Status: {response.Payment.Status}"
                    });
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error during payment processing: {Message}", ex.Message);

                var errorMessage = GetUserFriendlyErrorMessage(ex);
                return BadRequest(new
                {
                    Status = "Failed",
                    Message = errorMessage
                });
            }
        }

        [HttpGet("create-payment-link")]
        public async Task<IActionResult> CreatePaymentLink()
        {

            try
            {
                var idempotencyKey = Guid.NewGuid().ToString();
                var money = new Money
                {
                    Amount = 10000,
                    Currency = Currency.Gbp
                };


                var locationsResponse = await _squareClient.Locations.ListAsync();
                var locationId = locationsResponse.Locations?.FirstOrDefault(l => l.Status == LocationStatus.Active)?.Id;
                if (string.IsNullOrEmpty(locationId))
                {
                    return BadRequest(new { Status = "Failed", Message = "No active Square location found." });
                }

                var quickPay = new QuickPay
                {
                    Name = "TestPayment",         // Required
                    PriceMoney = money,        // Required
                    LocationId = locationId,

                };

                var request = new CreatePaymentLinkRequest
                {
                    IdempotencyKey = idempotencyKey,
                    QuickPay = quickPay,
                    Description = "Test test",
                    CheckoutOptions = new CheckoutOptions
                    {
                        RedirectUrl = "http://127.0.0.1"
                    },


                };

                var response = await _squareClient.Checkout.PaymentLinks.CreateAsync(request);

                if (response.PaymentLink != null)
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Url = response.PaymentLink.Url,
                        LongUrl = response.PaymentLink.LongUrl
                    });
                }
                else
                {
                    return BadRequest(new { Status = "Failed", Errors = response.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment link");
                return StatusCode(500, new { Status = "Failed", Message = ex.Message });
            }
        }


        private string GetUserFriendlyErrorMessage(Exception ex)
        {

            var message = ex.Message.ToLower();

            if (message.Contains("card_declined"))
            {
                return "Your card was declined. Please try a different card.";
            }
            else if (message.Contains("insufficient_funds"))
            {
                return "Insufficient funds on your card.";
            }
            else if (message.Contains("expired_card"))
            {
                return "Your card has expired. Please use a different card.";
            }
            else if (message.Contains("invalid_expiration"))
            {
                return "Invalid card expiration date.";
            }
            else if (message.Contains("invalid_card"))
            {
                return "Invalid card information. Please check your card details.";
            }
            else if (message.Contains("verify_needed"))
            {
                return "Card verification required. Please contact your bank.";
            }
            else if (message.Contains("authentication_required"))
            {
                return "Card authentication required. Please try again.";
            }
            else
            {
                return "Payment processing failed. Please try again.";
            }
        }
    }
}











