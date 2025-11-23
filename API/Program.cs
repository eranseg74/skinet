using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

// This line is the seperator between service configuration and app configuration. Everything above this line is configuring services, everything below is configuring the app and this is where we will configure the middlewares.
// Services are anythimg we will inject into other parts of the application via Dependency Injection (DI). Middlewares are components that form the request pipeline and handle requests and responses.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();
