using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cityton.Repository;
using Cityton.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Cityton.Data;
using FluentValidation.AspNetCore;
using FluentValidation;
using Cityton.Data.Models;
using Cityton.Service.Validators;
using Cityton.Data.DTOs;
using Cityton.Service.Validators.DTOs;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace Cityton.Ui
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

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));
            services.AddDbContext<ApplicationContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<ApplicationContext>(x => x.UseMySql("server=remotemysql.com;user id=ol2EsK1Yz9;password=OOJ79dvEZa;port=3306;database=ol2EsK1Yz9;"));
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("Settings");
            services.Configure<Settings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<Settings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/chat")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddMemoryCache();

            // configure DI for application services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(ICompanyRepository), typeof(CompanyRepository));
            services.AddScoped(typeof(IGroupRepository), typeof(GroupRepository));
            services.AddScoped(typeof(IParticipantGroupRepository), typeof(ParticipantGroupRepository));
            services.AddScoped(typeof(IChallengeRepository), typeof(ChallengeRepository));
            services.AddScoped(typeof(IMesageRepository), typeof(MesageRepository));
            services.AddScoped(typeof(IDiscussionRepository), typeof(DiscussionRepository));
            services.AddScoped(typeof(IChallengeGivenRepository), typeof(ChallengeGivenRepository));
            services.AddScoped(typeof(IUserInDiscussionRepository), typeof(UserInDiscussionRepository));
            services.AddScoped(typeof(IMediaRepository), typeof(MediaRepository));

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IChallengeService, ChallengeService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddHttpContextAccessor();

            services.AddControllers().AddFluentValidation();

            services.AddSignalR();

            services.AddTransient<IValidator<Company>, CompanyValidator>();
            services.AddTransient<IValidator<User>, UserValidator>();
            services.AddTransient<IValidator<Challenge>, ChallengeValidator>();
            services.AddTransient<IValidator<Achievement>, AchievementValidator>();
            services.AddTransient<IValidator<Group>, GroupValidator>();
            services.AddTransient<IValidator<ParticipantGroup>, ParticipantGroupValidator>();
            services.AddTransient<IValidator<ChallengeGiven>, ChallengeGivenValidator>();
            services.AddTransient<IValidator<Discussion>, DiscussionValidator>();
            services.AddTransient<IValidator<UserInDiscussion>, UserInDiscussionValidator>();
            services.AddTransient<IValidator<Message>, MessageValidator>();
            services.AddTransient<IValidator<Media>, MediaValidator>();
            services.AddTransient<IValidator<RegisterDTO>, RegisterDTOValidator>();
            services.AddTransient<IValidator<LoginDTO>, LoginDTOValidator>();
            services.AddTransient<IValidator<UserUpdateDTO>, UserUpdateDTOValidator>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseWebSockets();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hub/chatHub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                // if (env.IsDevelopment())
                // {
                //     spa.UseAngularCliServer(npmScript: "start");
                // }
            });
        }
    }
}
