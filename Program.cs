using BookstoreWeb.Application;
using BookstoreWeb.Infrastructure;

var builder=WebApplication.CreateBuilder(args);

//service từ App layer
builder.Services.AddApplicationServices();

//rpeo từ Infras layer
//phase 4 điền
builder.Services.AddInfrastructureServices();

//cho .NET tìm + xài Controller class trong API
builder.Services.AddControllers();

var app=builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
