using DriveX_Backend.DB;
using DriveX_Backend.IRepository;
using DriveX_Backend.IServices;
using DriveX_Backend.Repository;
using DriveX_Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace DriveX_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure DbContext with SQL Server
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DBConection")));

            // Register services and repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserServices>();
            builder.Services.AddScoped<IModelService, ModelService>();


            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IModelRepository, ModelRepository>();

            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<ICarService, CarService>();  


            // Optional: Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
