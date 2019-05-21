using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JWTWebApi.ActionFilters;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JWTWebApi.Contracts;
using LoggerService;

namespace JWTWebApi.Models
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
        }
        public static IServiceCollection ConfigureSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "ED&F REST API",
                    Version = "v1",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "none",
                    Contact = new Contact{
                        Name = "Cygnet Infotech Pvt Ltd",
                        Email = "info@cygnetinfotech.com",
                        Url = "https://cygnet-infotech.com"
                    },
                    License = new License{
                        Name = "Cygnet Infotech Pvt Ltd",
                        Url = "https://www.cygnet-infotech.com/terms-conditions"
                    }
                });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer",Enumerable.Empty<string>()},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme{
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(security);
                // c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.SchemaFilter<SwaggerExcludeFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
 
            return services;
        }
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "http://localhost:5001",
                    ValidAudience = "http://localhost:5001",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
                };
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json","ED&F REST API V1");
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet("/swagger/ui/custome.css");
                // c.DocExpansion(docExpansion.none);
            });
 
            return app;
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
    }
}