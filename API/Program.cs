using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adding the controllers service to the DI container
builder.Services.AddControllers();
// Adding the DbContext as a service so we will be able to inject it to other classes.
// The type of the DbContext will be the StoreContext that we created in the Infrastructure folder. This class will contain all the DbSets (tables) we need for the application.
// The opt is the options we pass to the DbContext. The UseSqlServer method configures the context to connect to a SQL Server database. The builder.configuration gives access to the configuration file () and the GetConnectionString method is a specific method for reading the coneection string from the configuration file. We set the name of the connection string to DefaultConnections. This is the key and the value is the connection string to the SQL Server itself.
builder.Services.AddDbContext<StoreContext>(opt =>
{
  opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adding the ProductRepository as a service so we can inject it to other classes via DI.
// Using the AddScoped method means that a new instance of the ProductRepository will be created for each HTTP request and will be shared across the different components that are part of that request. This is suitable for repositories that interact with the DbContext, as it ensures that all operations within a single request use the same context instance, maintaining consistency and enabling features like change tracking.
// When a request comes it will go to the appropriate controller, and if that controller has a constructor parameter of type IProductRepository, the DI container will provide an instance of ProductRepository. The controller can then use this instance to perform data operations related to products. Once the request is completed, the instance will be disposed of.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// Other types of service lifetimes include AddSingleton (a single instance is created and shared throughout the application's lifetime) and AddTransient (a new instance is created each time the service is requested and is disposed once the method is completed).

// When registering the Generic repository service, because the type is unknown (type T can be any type) the syntax will be as follows:
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding CORS to the application. CORS is a security feature in browsers. Cross-Origin Resource Sharing (CORS) is an HTTP-header based mechanism that allows a server to indicate any origins (domain, scheme, or port) other than its own. CORS works by adding Access-Control-Allow-* headers to your server's responses. These headers inform the browser which origins (domains), methods (GET, POST, PUT, DELETE), and headers are permitted for cross-origin requests.
// After defining it here as a service we need to define its middleware. It must be between the exception and the controll mapping middlewares otherwise it might not work!
builder.Services.AddCors();

builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
  var connString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connection string");
  var configuration = ConfigurationOptions.Parse(connString, true);
  return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<ICartService, CartService>();

// Adding services for Identity Framework
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>().AddEntityFrameworkStores<StoreContext>();

builder.Services.AddSignalR();

// This line is the seperator between service configuration and app configuration. Everything above this line is configuring services, everything below is configuring the app and this is where we will configure the middlewares.
// Services are anythimg we will inject into other parts of the application via Dependency Injection (DI). Middlewares are components that form the request pipeline and handle requests and responses.

var app = builder.Build();

// Configure the HTTP request pipeline.
// MIDDLEWARES
app.UseMiddleware<ExceptionMiddleware>();

// Adding the CORS middleware to the pipeline. Adding the AllowCredentials() method will allow the cookies to be passed along with the request. This is necessary for authentication because the cookie contains the authentication token. This will allow sending cookies from the client (Angular) to the server (API) which are on different domains.
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "https://localhost:4200")); // In the method we need to define what are we allowing. In the WithOrigins we are specifying from where we are allowing the requests to come from. Without these URLs the request will still go to the server but it is the browser that will not display the content (browser security feature)

// Specifying middleware for SignalR authentication because SignalR does not use the API endpoints. Must be in this order. Also, because we are using cookies this will work without any special configuration
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Adding configuration for the Identity Framework
app.MapGroup("api").MapIdentityApi<AppUser>(); // The MapGroup allows to add text to the url. Without it the url will be composed of the baseUrl and the login/reister words that come from the IdentityFramework (would be - https://localhost:5001/login). defining "api" in the MapGroup will add the "api" word -> https://localhost:5001/api/login (because outr login and register implementation is in the AccountController the url will be -> https://localhost:5001/api/account/login)

app.MapHub<NotificationHub>("/hub/notifications"); // Maps incoming requests with the specified path to the specified Microsoft.AspNetCore.SignalR.Hub type.

// Seeding data if needed (adding initial data to the database in case it is empty)
try
{
  // Creating a scope to get the StoreContext service from the DI container. This is necessary because the StoreContext is registered with a scoped lifetime, meaning it is created per request. Since we are outside of an HTTP request context here, we need to create a scope manually to access the StoreContext. When we create a scope, we are essentially creating a new context for dependency injection that allows us to resolve scoped services. After we create the scope, we can request the StoreContext service from the service provider within that scope. When the seeding operation is complete, the scope is disposed of, which also disposes of any scoped services created within it, including the StoreContext.
  using var scope = app.Services.CreateScope();
  var services = scope.ServiceProvider;
  var context = services.GetRequiredService<StoreContext>();
  await context.Database.MigrateAsync(); // Applying any pending migrations to the database. This ensures that the database schema is up to date with the current model defined in the application before seeding data.
  await StoreContextSeed.SeedAsync(context); // Calling the SeedAsync method from the StoreContextSeed class to seed the database with initial data if needed.
}
catch (Exception ex)
{
  System.Console.WriteLine($"Error during migration: {ex.Message}");
  throw;
}

app.Run();
