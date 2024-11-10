

using ExpenseTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace ExpenseTrackerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
          //  var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            // Add services to the container.
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(name: MyAllowSpecificOrigins,
            //                      policy =>
            //                      {
            //                          policy.WithOrigins("http://localhost:3000",
            //                                              "http://192.168.1.109:3000")
            //                          .AllowAnyHeader()
            //                                      .AllowAnyMethod();
                                     
            //                      });
            //});
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var app = builder.Build();
            app.UseCors("MyPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
           
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}