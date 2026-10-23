
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SchoolApp.Configuration;
using SchoolApp.Data;
using SchoolApp.Repositories;
using SchoolApp.Security;
using SchoolApp.Services;
using Serilog;
using System.Text;
using System.Reflection;
using System.Text.Json.Serialization;
using SchoolApp.Helpers;


namespace SchoolApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connString = builder.Configuration.GetConnectionString("DevConnection");
            builder.Services.AddDbContext<SchoolRest10Context>(options => options.UseSqlServer(connString));

            builder.Host.UseSerilog((hostingContext, configuration) =>
            {
                configuration.ReadFrom.Configuration(hostingContext.Configuration);
            });

            builder.Services.AddRepositories();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = builder.Environment.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowCFClient", policy => policy.WithOrigins(builder.Configuration["Cors:Origin"]!)
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "School App", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authentication header using the Bearer Scheme",
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
                options.OperationFilter<AuthorizeOperationFilter>();
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "School App v1"));
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "School App v2"));
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowCFClient");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
