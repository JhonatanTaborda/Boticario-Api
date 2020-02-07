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

namespace Boticario.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
                
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


            services.AddSwaggerDocument(config => {
                config.DocumentName = "Boticario.Api";
            });
        }
                
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseDeveloperExceptionPage();
                       
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc();


            app.UseSwagger(config => {
                config.DocumentName = "Boticario.Api";
                config.Path = "/api/swagger.json";
                config.PostProcess = (document, request) => { };
            });

            app.UseSwaggerUi3(config => {
                config.DocumentPath = "/api/swagger.json";
                config.Path = "/api/swagger";
            });            
        }
    }
}
