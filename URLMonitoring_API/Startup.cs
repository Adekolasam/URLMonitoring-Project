using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URLMonitoring_API.Data;
using URLMonitoring_API.Model;

namespace URLMonitoring_API
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
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Connection")));
            services.AddSwaggerGen();
            //services.AddAutoMapper(typeof(Program));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers().AddNewtonsoftJson();

            //var paginationDefaultParameter = new PaginationDefaultParameter
            //{
            //    MaxPageSize = int.Parse(Configuration["Pagination:MaxPageSize"]),
            //    PageSize = int.Parse(Configuration["Pagination:PageSize"])
            //};
            //services.AddSingleton(paginationDefaultParameter);

            //services.AddSingleton<IUriService>(o =>
            //{
            //    var accessor = o.GetRequiredService<IHttpContextAccessor>();
            //    var request = accessor.HttpContext.Request;
            //    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
            //    return new UriService(uri);
            //});

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .WithOrigins(Configuration["AllowedDomain:CALANGWEB"])
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("X-Pagination");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

            app.UseRouting();
            app.UseCors(); //for api call

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
