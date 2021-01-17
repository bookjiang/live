using live.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySQL.Data.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.IO;

namespace live
{
    public class Startup
    {
        public Startup(IConfiguration configuraton)
        {
            Configuration = configuraton;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LiveMultiContext>(opt =>
            {
                opt.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();

            ////设置跨域处理
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("any", builder =>
            //    {
            //        builder.AllowAnyOrigin()//允许任何来源的主机访问
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials();//指定处理cookie

            //    });
            //});
            //注意：.net Core 3.1版本  Cors配置不能同时启用  AllowAnyOrigin()  .AllowAnyMethod()  .AllowAnyHeader()  .AllowCredentials()

            //添加cors 服务 配置跨域处理   
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                    //.AllowCredentials()//指定处理cookie
                .AllowAnyOrigin(); //允许任何来源的主机访问
                });
            });


            //添加swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API ", Version = "v1" });
                
                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "live.xml");
                var filePath = Path.Combine(System.Environment.CurrentDirectory, "live.xml");

                c.IncludeXmlComments(filePath);


            });

            //services.AddSwaggerGenNewtonsoftSupport();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //设置cors
            app.UseCors("any");

            //添加Swagger有关中间件
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

            });

            app.UseRouting(); 
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
