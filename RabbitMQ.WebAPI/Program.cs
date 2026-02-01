using Microsoft.Extensions.Options;

namespace RabbitMQ.WebAPI
{
    public class Program
    {
        const string MyCorsPolicy = "_myCorsPolicy";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddCors(opt => opt.AddPolicy(name: MyCorsPolicy, policy =>
            {
                policy
                    .WithOrigins("http://localhost:801")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthorization();
            app.UseCors(MyCorsPolicy);
            app.MapControllers();
            app.Run();
        }
    }
}
