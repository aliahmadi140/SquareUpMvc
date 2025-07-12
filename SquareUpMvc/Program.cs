using Square;

namespace SquareUpMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get Square configuration
            var squareAccessToken = builder.Configuration["Square:AccessToken"];
            var squareEnvironment = builder.Configuration["Square:Environment"];

            // Validate configuration
            if (string.IsNullOrEmpty(squareAccessToken))
            {
                throw new InvalidOperationException("Square:AccessToken is not configured in appsettings.json");
            }

            // Configure Square client options
            var clientOptions = new ClientOptions
            {
                BaseUrl = squareEnvironment?.ToLower() == "production" 
                    ? SquareEnvironment.Production 
                    : SquareEnvironment.Sandbox
            };

            // Create and register SquareClient with DI container
            var squareClient = new SquareClient(squareAccessToken, clientOptions);
            builder.Services.AddSingleton(squareClient);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            
            // Add CORS for API calls
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
