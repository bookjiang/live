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
using Microsoft.AspNetCore.Cors;

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

            ////���ÿ�����
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("any", builder =>
            //    {
            //        builder.AllowAnyOrigin()//�����κ���Դ����������
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials();//ָ������cookie

            //    });
            //});
            //ע�⣺.net Core 3.1�汾  Cors���ò���ͬʱ����  AllowAnyOrigin()  .AllowAnyMethod()  .AllowAnyHeader()  .AllowCredentials()

            //���cors ���� ���ÿ�����   
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                    //.AllowCredentials()//ָ������cookie
                .AllowAnyOrigin(); //�����κ���Դ����������
                });
            });


            //���swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API ", Version = "v1" });
                
                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "live.xml");
                var filePath = Path.Combine(System.Environment.CurrentDirectory, "live.xml");

                c.IncludeXmlComments(filePath,true);


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



            //���Swagger�й��м��
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

            });

            app.UseRouting();
            //����cors,//��Ҫ���������У�����Ҫע�⣬��һ��Ҫ��app.UseRouting �� UseEndpoints ֮��
            app.UseCors("any");
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
