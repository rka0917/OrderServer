using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OrderServer.Model;
using OrderServer.Service;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddDbContext<OrderContext>(opt => opt.UseInMemoryDatabase("OrderService"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// For future work, we can make sure to add swagger documentation. Would be useful for listing enum types for example. 
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// For future work, extra necessary middleware need to be added, which is out of scope for this project atm. 
// For instance, authentication and authorization management with Azure AD or with the use of APIKeys for example. 

// Another assumption made is that this is not information exposed to the customer.
// The decisions made are assuming that this is information that only a developer or an admin should have access to. 
// That is why auto incremented int-ids are passed through the API. Otherwise, I might have considered using GUIDS instead and why it returns exceptions in some of the calls.  
// Of course, for future work, the in-memory implementation should be switched out to a real db implementation. Maybe configure so it uses in-memory for development environments or during testing. 