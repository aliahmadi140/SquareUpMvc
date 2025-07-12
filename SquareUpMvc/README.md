# SquareUpMvc - Square Payment Integration

A simple ASP.NET Core MVC application that demonstrates how to integrate Square payments into a web application.

## Features

- **Square Web Payments SDK Integration**: Secure card tokenization on the frontend
- **ASP.NET Core API**: Backend payment processing with Square API
- **Modern UI**: Bootstrap-based responsive design
- **Error Handling**: Comprehensive error handling for payment failures
- **Logging**: Detailed logging for debugging and monitoring

## Prerequisites

- .NET 8.0 SDK
- Square Developer Account
- Square Application ID and Location ID
- Square Access Token

## Configuration

1. **Square Developer Dashboard Setup**:
   - Create a Square Developer account
   - Create a new application
   - Get your Application ID and Location ID
   - Generate an Access Token

2. **Update Configuration**:
   Edit `appsettings.json`:
   ```json
   {
     "Square": {
       "AccessToken": "YOUR_SQUARE_ACCESS_TOKEN",
       "Environment": "Sandbox"
     }
   }
   ```

3. **Update Frontend Credentials**:
   Edit `Views/Home/Index.cshtml`:
   ```javascript
   const appId = 'YOUR_APPLICATION_ID';
   const locationId = 'YOUR_LOCATION_ID';
   ```

## Running the Application

1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Run the Application**:
   ```bash
   dotnet run
   ```

3. **Access the Application**:
   Navigate to `https://localhost:5001` or `http://localhost:5000`

## Testing Payments

### Sandbox Test Cards

Use these test card numbers for testing:

- **Successful Payment**: `4111 1111 1111 1111`
- **Declined Payment**: `4000 0000 0000 0002`
- **Insufficient Funds**: `4000 0000 0000 9995`
- **Expired Card**: `4000 0000 0000 0069`

### Test Card Details

- **Expiration Date**: Any future date (e.g., `12/25`)
- **CVV**: Any 3 digits (e.g., `123`)
- **Postal Code**: Any valid postal code (e.g., `12345`)

## Project Structure

```
SquareUpMvc/
├── Controllers/
│   ├── HomeController.cs          # Main page controller
│   └── PaymentController.cs       # Payment API controller
├── Models/
│   ├── PaymentRequestModel.cs     # Payment request model
│   └── ErrorViewModel.cs          # Error handling model
├── Views/
│   └── Home/
│       └── Index.cshtml           # Payment form UI
├── Program.cs                     # Application startup
├── appsettings.json              # Configuration
└── SquareUpMvc.csproj            # Project file
```

## API Endpoints

### POST /api/payment/process

Process a payment with Square.

**Request Body**:
```json
{
  "sourceId": "token_from_square_js",
  "amount": 1000,
  "currency": "USD"
}
```

**Response**:
```json
{
  "status": "Success",
  "paymentId": "payment_id_from_square",
  "amount": 1000,
  "currency": "USD"
}
```

## Security Considerations

- **Access Token**: Store your Square access token securely (use environment variables in production)
- **HTTPS**: Always use HTTPS in production
- **Input Validation**: All inputs are validated on both client and server
- **Error Handling**: Sensitive information is not exposed in error messages

## Production Deployment

1. **Environment Variables**: Use environment variables for sensitive configuration
2. **HTTPS**: Ensure HTTPS is enabled
3. **Logging**: Configure proper logging for production
4. **Monitoring**: Set up monitoring for payment failures
5. **Square Production**: Switch to Square production environment

## Troubleshooting

### Common Issues

1. **"Failed to load Square.js"**:
   - Check your Application ID and Location ID
   - Ensure you're using the correct Square environment (sandbox/production)

2. **Payment Declined**:
   - Use the correct test card numbers
   - Check the error messages for specific reasons

3. **CORS Errors**:
   - Ensure the CORS policy is properly configured
   - Check that the API endpoint is accessible

### Debugging

- Check browser console for JavaScript errors
- Check application logs for backend errors
- Use Square Developer Dashboard to monitor API calls

## License

This project is for educational purposes. Please ensure compliance with Square's terms of service for production use. 