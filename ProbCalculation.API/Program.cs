using Calculation.Core.Service.Probability;
using Common.Logging.Service;
using Common.Validator.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<IProbCalculationService, ProbCalculationService>();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();
builder.Services.AddSingleton<IActivityLoggerService, ActivityLoggerService>();

// Configure CORS - allow your frontend origins (development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDevPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000") // <- React origin(s)
            .AllowAnyHeader()
            .AllowAnyMethod(); // allows OPTIONS, POST, GET etc.
                               // .AllowCredentials(); // enable only if you need cookies/auth (and then don't use AllowAnyOrigin)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("LocalDevPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
