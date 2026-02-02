using Hospital.Business.Services.Auth.Abstract;
using Hospital.Business.Services.Auth.Concrete;
using Hospital.DataAccess;
using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.DataAccess.Repositories.Auth.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<HospitalDbContext>(options =>
{
    options.UseSqlServer(conn);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


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
