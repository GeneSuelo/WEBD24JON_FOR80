using Microsoft.AspNetCore.Mvc.Formatters;
using Northwind.EntityModels; //AddNorthwindContext
using Microsoft.Extensions.Caching.Memory; // F�r att anv�nda IMemoryCache
using Northwind.WebApi.Repositories; // F�r att anv�nda ICustomerRepository
using Swashbuckle.AspNetCore.SwaggerUI; // F�r att anv�dna SubmitMethod

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMemoryCache>(
    new MemoryCache(new MemoryCacheOptions())); //L�gg till IMemoryCache
// Add services to the container.
builder.Services.AddNorthwindContext();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddControllers(options =>
{
    Console.WriteLine("Default output formatters: ");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter is null)
        {
            Console.WriteLine($"{formatter.GetType().Name}");
        }
        else //OutputFormatter klass anv�ndaer SupportedMediaTypes
        {
            Console.WriteLine("{0}, Media types: {1}",
                arg0: mediaFormatter.GetType().Name,
                arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
    }
}
); //Anv�nder JSON som standart, s� ingen extra konfiguration beh�vs h�r

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
            "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[]
        {
            SubmitMethod.Get, SubmitMethod.Post,
            SubmitMethod.Put, SubmitMethod.Delete });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
