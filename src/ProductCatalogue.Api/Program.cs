using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;
using ProductCatalogue.Api.Features.Products.CreateProduct;
using ProductCatalogue.Api.Features.Products.GetProduct;
using ProductCatalogue.Api.Features.Products.GetProducts;
using ProductCatalogue.Api.Features.Products.UpdateProduct;
using ProductCatalogue.Api.Features.Products.DeleteProduct;
using ProductCatalogue.Api.Infrastructure.Http;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProductCataloguePersistence(builder.Configuration);
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapCreateProductEndpoint();
app.MapGetProductEndpoint();
app.MapGetProductsEndpoint();
app.MapUpdateProductEndpoint();
app.MapDeleteProductEndpoint();

app.Run();

public partial class Program { }
