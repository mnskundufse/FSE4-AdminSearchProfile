using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Admin.SearchProfileService.Business.Contracts;
using Admin.SearchProfileService.Business.Implementation;
using Admin.SearchProfileService.Middleware;
using Admin.SearchProfileService.Models;
using Admin.SearchProfileService.Repository.Contracts;
using Admin.SearchProfileService.Repository.Implementation;
using Admin.SearchProfileService.Service;
using Admin.SearchProfileService.ServiceDiscovery;
using System;

namespace Admin.SearchProfileService
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

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<ISearchProfileBusiness, SearchProfileBusiness>();

            var config = new ServerConfig();
            Configuration.Bind(config);
            var customerContext = new SearchProfileContext(config.MongoDB);
            var repo = new SearchProfileRepository(customerContext);
            services.AddSingleton<ISearchProfileRepository>(repo);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("consumer", consumerConfig);

            services.AddSingleton<ConsumerConfig>(consumerConfig);

            services.AddHostedService<AdminSearchProfileService>();
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddConsulConfig(Configuration);

            var contact = new OpenApiContact()
            {
                Name = "Manas Kundu",
                Email = "mnskundu@gmail.com",
                Url = new Uri("http://www.example.com")
            };

            var license = new OpenApiLicense()
            {
                Name = "My License",
                Url = new Uri("http://www.example.com")
            };

            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "Swagger API: Search Full Stack Engineer Profile",
                Description = "Admin Functionality: As a Admin, User can list all the details about FSE Engineer.",
                TermsOfService = new Uri("http://www.example.com"),
                Contact = contact,
                License = license
            };

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", info);

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
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

            app.UseRouting();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseConsul(Configuration);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                "Swagger API: Search Full Stack Engineer Profile v1");
            });
        }
    }
}