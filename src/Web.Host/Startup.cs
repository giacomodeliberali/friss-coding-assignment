using System;
using System.IO;
using System.Net;
using System.Reflection;
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

            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IWebAssemblyMarker).Assembly));

            // Register custom objects to the DI container
            services.AddApplicationServices();
            services.AddRepositories();

            // Add dbcontext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("Default"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    context.Response.StatusCode = exceptionHandlerPathFeature?.Error switch
                    {
                        ValidationException => (int) HttpStatusCode.BadRequest,
                        BusinessException => (int) HttpStatusCode.InternalServerError,
                        _ => context.Response.StatusCode
                    };

                    await context.Response.WriteAsJsonAsync(new ExceptionDto
                    {
                        Name = exceptionHandlerPathFeature?.Error.GetType().Name,
                        StackTrace = env.IsDevelopment() ? exceptionHandlerPathFeature?.Error.StackTrace : null,
                        Message = exceptionHandlerPathFeature?.Error.Message
                    });
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
