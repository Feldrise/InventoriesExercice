using InventoriesExercise.API.Services;
using InventoriesExercise.API.Services.Interfaces;
using InventoriesExercise.API.Settings;
using InventoriesExercise.API.Settings.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoriesExercise.API
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
            services.Configure<MongoSettings>(Configuration.GetSection(nameof(MongoSettings)));
            services.Configure<InventoriesExerciseSettings>(Configuration.GetSection(nameof(InventoriesExerciseSettings)));

            services.AddSingleton<IMongoSettings>(Span => Span.GetRequiredService<IOptions<MongoSettings>>().Value);
            services.AddSingleton<IInventoriesExerciseSettings>(Span => Span.GetRequiredService<IOptions<InventoriesExerciseSettings>>().Value);

            // JWT Authentication
            var InventoriesExerciseSettings = Configuration.GetSection(nameof(InventoriesExerciseSettings)).Get<InventoriesExerciseSettings>();
            var key = Encoding.ASCII.GetBytes(InventoriesExerciseSettings.ApiSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IInventoriesService, InventoriesService>();
            services.AddScoped<IItemsService, ItemsService>();

            services.AddCors(options =>
            {
                options.AddPolicy("developerPolicy", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "InventoriesExercise.API",
                    Description = "An API to manage stocks, used as an exercice correction",
                    Contact = new OpenApiContact
                    {
                        Name = "Victor (Feldrise) DENIS",
                        Email = "contact@feldrise.com",
                        Url = new Uri("https://feldrise.com")
                    }
                });

                var filepath = Path.Combine(System.AppContext.BaseDirectory, "InventoriesExercise.API.xml");
                c.IncludeXmlComments(filepath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("developerPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoriesExercise.API v1");
                c.RoutePrefix = "documentation";
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
