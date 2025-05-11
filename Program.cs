using bekokkonen.pro.Global.Config;
using bekokkonen.pro.MQ.Implementation;
using bekokkonen.pro.Routes.Hubs;
using bekokkonen.pro.Routes.MapEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace bekokkonen.pro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            GlobalConfig.ApiDocumentConfig = builder.Configuration.GetRequiredSection("ApiDocument").Get<GlobalConfig.ApiDocument>()!;
            GlobalConfig.RabbitMQConfig = builder.Configuration.GetRequiredSection("RabbitMQ").Get<GlobalConfig.RabbitMQ>()!;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddAuthorization();

            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/electricity/consumption"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<ConsumptionHub>();
            builder.Services.AddSingleton<MQClient>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(["http://localhost:5173", "https://kokkonen.pro:443", "http://192.168.1.38:5173"])
                          .WithMethods("GET")
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.MapPingEndpoints();
            app.MapElectricityEndpoints();

            app.Run();
        }
    }
}
