using DriveInsight.Api.Data;
using DriveInsight.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string ReactDevelopmentCorsPolicy = "ReactDevelopment";

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DriveInsightDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactDevelopmentCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IDriveAnalysisService, DriveAnalysisService>();
builder.Services.AddScoped<IDriveService, DriveService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(ReactDevelopmentCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
