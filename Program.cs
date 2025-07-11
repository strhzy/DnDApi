using Microsoft.EntityFrameworkCore;
using DnDAPI.Models;
using DnDAPI.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DnDContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "DndPartyManager API";
        document.Info.Version = "v1";
    };
});

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();