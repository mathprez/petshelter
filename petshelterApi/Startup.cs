using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using petshelterApi.Database;
using petshelterApi.Services;
using petshelterApi.Services.Authorization;
using System;
using System.Net.Http.Headers;

namespace petshelterApi
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
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
            });

            services.AddHttpClient<IAuth0ApiClient, Auth0ApiClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Auth0:ManagementDomain"]);
                client.DefaultRequestHeaders.Add("authorization", Configuration["Auth0:ManagementToken"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddHttpClient<DogImageApiClient>();
            services.AddHttpClient<CatImageApiClient>();
            
            services.AddDbContext<PetShelterDbContext>(options => options.UseSqlServer(Configuration["Database:ConnectionString"]));

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddSingleton(Configuration);

            services.AddMediatR();
            services.AddAutoMapper();
            services.AddHttpContextAccessor();
            services.AddScoped<MailService>();
            services.AddScoped<UserResolverService>();
            services.AddScoped<ImageSaver>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IAuthorizationHandler, AppointmentAuthorizationCrudHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, PetShelterDbContext petShelterDbContext)
        {
            if (env.IsDevelopment())
            {
                //petShelterDbContext.Database.EnsureDeleted();
                if (petShelterDbContext.Database.EnsureCreated())
                {
                    petShelterDbContext.Seed();
                }

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
                builder.AllowCredentials();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
