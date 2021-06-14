using AuthenticationAPI.Models;
using AuthenticationAPI.Services;
using AuthenticationAPI.UserData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;


namespace AuthenticationAPI
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

            var audienceConfig = Configuration.GetSection("Audience");

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer("TestKey", x =>
         {
             x.RequireHttpsMetadata = false;
             x.TokenValidationParameters = tokenValidationParameters;
         });

            var connection = Configuration["MYSQL_DBCONNECTION"] ?? Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContextPool<UsersContext>(options => options.UseMySQL(connection));

            /*           services.AddDbContextPool<UsersContext>(options =>
                       {
                           string connectionString = Configuration.GetConnectionString("DefaultConnection");
                           options.UseMySql(connectionString,
                               ServerVersion.AutoDetect(connectionString),
                               mySqlOptions =>
                                   mySqlOptions.EnableRetryOnFailure(
                                       maxRetryCount: 10,
                                       maxRetryDelay: TimeSpan.FromSeconds(30),
                                       errorNumbersToAdd: null));
                       }
                   );*/

            /*  var connectionString = Configuration.GetConnectionString("DefaultConnection");

              // Replace with your server version and type.
              // Use 'MariaDbServerVersion' for MariaDB.
              // Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
              // For common usages, see pull request #1233.
              var serverVersion = new MySqlServerVersion(new Version(5, 7));

              // Replace 'YourDbContext' with the name of your own DbContext derived class.
              services.AddDbContext<UsersContext>(
                  dbContextOptions => dbContextOptions
                      .UseMySql(connectionString, serverVersion)
                      .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                      .EnableDetailedErrors()       // <-- with debugging (remove for production).
              );*/

            services.AddScoped<IUserData, SqlUserData>();

            services.AddTransient<IMessagePublisher, MessagePublisher>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthenticationAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UsersContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthenticationAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            context.Database.Migrate();
        }
    }
}


