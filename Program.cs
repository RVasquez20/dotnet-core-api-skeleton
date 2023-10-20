using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Utilities_Net_6_MVC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Fabrica de HttpClient
builder.Services.AddHttpClient("Base", client =>
{
    client.BaseAddress = new Uri("url");
    client.Timeout = TimeSpan.FromSeconds(120);
}).AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
            .RetryAsync(3))
        .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(45)));

//Configuracion de Inyeccion de dependencias para bbdd
//builder.Services.AddDbContext<Context>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ConectionString"), option => option.EnableRetryOnFailure());
//});
//Configuracion de Inyeccion de dependencias para patron repositorio
//builder.Services.AddScoped<IAppDbContext, Context>();
//builder.Services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

//Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
