using Microsoft.AspNetCore.Mvc.Formatters;
using Northwind.EntityModels; //AddNorthwindContext
using Microsoft.Extensions.Caching.Memory; // För att använda IMemoryCache

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMemoryCache>(
    new MemoryCache(new MemoryCacheOptions())); //Lägg till IMemoryCache
// Add services to the container.
builder.Services.AddNorthwindContext();

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
        else //OutputFormatter klass användaer SupportedMediaTypes
        {
            Console.WriteLine("{0}, Media types: {1}",
                arg0: mediaFormatter.GetType().Name,
                arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
    }
}
)
    .AddXmlDataContractSerializerFormatters() 
    .AddXmlSerializerFormatters();//Lägg till XML formatter

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
