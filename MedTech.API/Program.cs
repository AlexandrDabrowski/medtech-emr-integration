using MedTech.Core.Handlers;
using MedTech.Core.Interfaces;
using MedTech.Core.Services;
using MedTech.Infrastructure.Data;
using MedTech.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MedTechContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
builder.Services.AddScoped<IIntegrationLogRepository, IntegrationLogRepository>();

builder.Services.AddScoped<IPointsCalculator, StandardPointsCalculator>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();

builder.Services.AddScoped<CreatePatientHandler>();
builder.Services.AddScoped<UpdatePatientHandler>();
builder.Services.AddScoped<RecordProcedureHandler>();
builder.Services.AddScoped<GetPatientHandler>();
builder.Services.AddScoped<GetPatientHistoryHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevPolicy");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MedTechContext>();
    await context.Database.MigrateAsync();
}

app.Run();