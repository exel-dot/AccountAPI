using AccountAPI.Entities;
using AccountAPI.Middleware;
using AccountAPI.Models;
using AccountAPI.Models.Validators;
using AccountAPI.Service;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System;
using System.Reflection;
using System.Text;

namespace AccountAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var authenticationSettings = new AuthenticationSettings();

            builder.Services.AddDbContext<AccountDbContext>();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);


            builder.Services.AddSingleton(authenticationSettings);
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });

            builder.Services.AddControllers().AddFluentValidation();
            builder.Services.AddControllers();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<AccountDbContext>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Host.UseNLog();
            builder.Services.AddScoped<ErrorHandlingMiddleware>();
            builder.Services.AddSwaggerGen();
           
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<AccountDbContext>();
               
                   dbContext.Database.Migrate();
                   dbContext.Database.EnsureCreated();

            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

           
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account API");
            } );
         
          
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
          
            
            app.Run();
        }
    }
}