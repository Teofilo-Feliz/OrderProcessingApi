
using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Application.Interfaces;
using OrderProcessingApi.Application.Services;
using OrderProcessingApi.Infrastructure.Configuration;
using OrderProcessingApi.Infrastructure.Repository.OrderRepository.Services;
using OrderProcessingApi.Infrastructure.Repository.ProductRepository.Services;
using OrderProcessingApi.Infrastructure.Repository.UnitOfWork.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderProcessingApiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrder, OrderService>();
builder.Services.AddScoped<IProduct, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
