using MockEMR.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IWebhookService, WebhookService>();
builder.Services.AddScoped<IWebhookService, WebhookService>();

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

app.Run();