using BroadYap.Hubs;
using BroadYap.DataService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<SharedDb>();
// Replace with builder pattern for different db connections later
builder.Services.AddSingleton<MySQLConnection>();

builder.Services.AddScoped<IUserRepository, MySQLUserRepository>();
builder.Services.AddScoped<IAuthOptionRepository, MySQLAuthOptionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, MySQLRefreshTokenRepository>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthenticationStrategy, PasswordStrategy>();
builder.Services.AddScoped<AuthenticationService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("dev", policy =>
        {
            policy
                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("dev");
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
