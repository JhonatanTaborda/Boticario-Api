using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Boticario.Api.Context;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Boticario.Api.Repository;
using NSwag.Generation.Processors.Security;
using System.Linq;
using NSwag;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Boticario.Api
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)////, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            ////_loggerFactory = loggerFactory;
        }        
                
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddEntityFrameworkSqlServer()                
                .AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services                
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ILoginRepository, LoginRepository>()
                .AddTransient<ISalesRepository, SalesRepository>();                                   

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                                    
            
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                };                
            }); 

            services.AddOpenApiDocument(config =>
            {
                config.DocumentName = "Boticario.Api";
                config.Title = "Boticario.Api";
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
                config.AddSecurity("Bearer", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = nameof(Authorization),
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Insira o Token: Bearer {token}"
                    }
                );
            });

        }
                
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                        ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole();
                       
            app.UseDeveloperExceptionPage();
                       
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc();            

            app.UseOpenApi();
            app.UseSwaggerUi3(config => {                
                config.Path = "/api/swagger";
            });                        
        }
    }
}
