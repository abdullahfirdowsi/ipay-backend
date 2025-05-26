// Program.cs (for .NET 6+ minimal hosting)
using Microsoft.EntityFrameworkCore;
using BillPaymentBackend.Data;
using BillPaymentBackend.Services;
using BillPaymentBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure HttpClient services
builder.Services.AddHttpClient<IRazorpayService, RazorpayService>();
builder.Services.AddHttpClient<IBharatBillPayService, MockBharatBillPayService>();

// Register BillPayment service
builder.Services.AddScoped<IBillPaymentService, BillPaymentService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Initialize the database
try
{
    await DbInitializer.Initialize(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while seeding the database.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
