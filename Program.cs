using bekokkonen.pro.Config;
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapPingEndpoints();
            app.MapElectricityEndpoints();

            app.Run();
        }
    }
}
