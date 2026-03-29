using BookstoreWeb.Application;
using BookstoreWeb.Infrastructure;

var builder=WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app=builder.Build();
if(app.Environment.IsDevelopment())
{
    //swagger thay razor views=giao diện test api
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();