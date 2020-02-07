using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), o => o.MigrationsAssembly("Boticario.Api"))
            //);

            services                
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ILoginRepository, LoginRepository>();


            services.AddSwaggerDocument(config => {
                config.DocumentName = "Boticario.Api";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
               
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }) //(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
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
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            }); 
        }
                
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseDeveloperExceptionPage();
            
            ////ConfigureOAuth(app);                        
            app.UseAuthentication();            
            app.UseIdentity();


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

        ////public void ConfigureOAuth(IApplicationBuilder app)
        ////{
        ////    OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
        ////    {
        ////        AllowInsecureHttp = true,
        ////        TokenEndpointPath = new PathString("/token"),
        ////        AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
        ////        Provider = new SimpleAuthorizationServerProvider()
        ////    };

        ////    // Token Generation
        ////    app.UseOAuthAuthorizationServer(OAuthServerOptions);
        ////    app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        ////}
    }
}
