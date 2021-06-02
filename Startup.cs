using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using webapi1.Helpers;
using webapi1.Models;

namespace webapi1
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

            services.AddDbContext<ContosouniversityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            // inject jwtHelpers
            services.AddSingleton<JwtHelpers>();


            // 注入OpenAPI v3.0
            // services.AddOpenApiDocument(document =>
            // {
            //     document.DocumentProcessors.Add(
            //     new SecurityDefinitionAppender("JWT",
            //     new OpenApiSecurityScheme
            //     {
            //         Type = OpenApiSecuritySchemeType.Http,
            //         Scheme = JwtBearerDefaults.AuthenticationScheme,
            //         Name = "Authorization",
            //         In = OpenApiSecurityApiKeyLocation.Header,
            //         Description = "Type into the textbox: Bearer {your JWT token}.",
            //     }));

            //     document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            // });

            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d =>
                {
                    d.Info.Title = "This is title";
                    d.Info.Contact = new OpenApiContact()
                    {
                        Email = "fly12610@gmail.com",
                        Name = "Junyu Wang",
                        Url = "https://github.com/qazs10015"
                    };
                    d.Info.Description = "This is my sample swagger";
                    d.Info.Version = "This is version";
                };
            });

            // 注入OpenAPI v3.0
            services.AddOpenApiDocument(config =>
            {
                // 這個 OpenApiSecurityScheme 物件請勿加上 Name 與 In 屬性，否則產生出來的 OpenAPI Spec 格式會有錯誤！
                var apiScheme = new OpenApiSecurityScheme()
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    // BearerFormat = "JWT", // for documentation purposes (OpenAPI only)
                    Description = "Copy JWT Token into the value field: {token}"
                };

                // 這裡會同時註冊 SecurityDefinition (.components.securitySchemes) 與 SecurityRequirement (.security)
                config.AddSecurity("Bearer", apiScheme);

                // 這段是為了將 "Bearer" 加入到 OpenAPI Spec 裡 Operator 的 security (Security requirements) 中
                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
            });



            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true; // Default: true

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Let "sub" assign to User.Identity.Name
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        // Let "roles" assign to Roles for [Authorized] attributes
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                        // Validate the Issuer
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                        ValidateAudience = false,
                        //ValidAudience = "JwtAuthDemo", // TODO

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = false,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                    };
                });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // 開啟openAPI功能
                app.UseOpenApi();
                // 開啟openAPI的swaggerUI介面
                app.UseSwaggerUi3();
                // 設定reDoc的路由，預設路徑會和swagger路徑衝突
                app.UseReDoc(config => config.Path = "/redoc");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
