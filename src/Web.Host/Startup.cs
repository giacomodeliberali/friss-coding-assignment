using System;
using System.IO;
using System.Net;
using Application;
using Application.Contracts;
using Domain.Exceptions;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Web.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // add controllers of the Web project
            services.AddControllers()
                .PartManager
                .ApplicationParts
                .Add(new AssemblyPart(typeof(IWebAssemblyMarker).Assembly));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web.Host", Version = "v1" });
                c.CustomSchemaIds(t => t.FullName);

                var path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory)!);
                foreach (var filePath in Directory.GetFiles(path, "*.xml"))
                {
                    c.IncludeXmlComments(filePath);
                }
            });

            // Register custom objects to the DI container
            services.AddApplicationServices();
            services.AddRepositories();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("Default"));
            });

            // Add caching
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web.Host v1");
                });
            }

            // Add serilog middleware
            app.UseSerilogRequestLogging();

            // Add unhandled exception middleware: serialize all as ExceptionDto with the appropriate status code
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    context.Response.StatusCode = exceptionHandlerPathFeature?.Error switch
                    {
                        ValidationException => (int) HttpStatusCode.BadRequest,
                        BusinessException => (int) HttpStatusCode.BadRequest,
                        _ => context.Response.StatusCode
                    };

                    // write exception details only in development
                    var serializedException = new ExceptionDto
                    {
                        Name = exceptionHandlerPathFeature?.Error.GetType().Name,
                        StackTrace = env.IsDevelopment() ? exceptionHandlerPathFeature?.Error.StackTrace : null,
                        Message = env.IsDevelopment() ? exceptionHandlerPathFeature?.Error.Message : null,
                    };

                    await context.Response.WriteAsJsonAsync(serializedException);
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
