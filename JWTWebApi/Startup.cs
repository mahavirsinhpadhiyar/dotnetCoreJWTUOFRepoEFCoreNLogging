using System.IO;
using JWTWebApi.Contracts;
using JWTWebApi.Entities;
using JWTWebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using TestCoreUow.UnitOfWork;

namespace JWTWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(System.String.Concat(Directory.GetCurrentDirectory(), "/nLog.config"));
            //older version
            //, ILoggerFactory loggerFactory in parameter in current class
            // loggerFactory.ConfigureNLog(System.String.Concat(Directory.GetCurrentDirectory(), "/nLog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //configure JWT authentication
            services.ConfigureAuthentication();

            //add framework services
            services.AddDbContext<RepositoryContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.ConfigureCors();
            services.ConfigureSwaggerDocumentation();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.ConfigureLoggerService();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("EnableCORS");
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            // app.UseExceptionHandler(config =>
            // {
            //     config.Run(async context =>
            //     {
            //         context.Response.StatusCode = 500;
            //         context.Response.ContentType = "application/json";
            //         context.Response.
 
            //         var error = context.Features.Get<IExceptionHandlerFeature>();
            //         if (error != null)
            //         {
            //             var ex = error.Error;
 
            //             await context.Response.WriteAsync(new ErrorModel()
            //             {
            //                 StatusCode = 500,
            //                 ErrorMessage = ex.Message 
            //             }.ToString()); //ToString() is overridden to Serialize object
            //         }
            //     });
            // });

            app.UseMvc();
        }
    }
}
