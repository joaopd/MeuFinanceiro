using Api.Extensions;
using Application.DependencyInjection;
using Infra;
using Infra.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAntiforgery();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("ALERTA: ConnectionString 'DefaultConnection' não encontrada.");
}

var app = builder.Build();
app.MigrateDatabase();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Meu Financeiro API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAntiforgery();
app.MapEndpoints(); 

app.Run();