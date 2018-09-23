using System.Text;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DutchTreat
{
   public class Startup
   {
      private IConfiguration _config;

      public Startup(IConfiguration config)
      {
         _config = config;
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddIdentity<StoreUser, IdentityRole>(cfg =>
         {
            cfg.User.RequireUniqueEmail = true;
         }).AddEntityFrameworkStores<DutchContext>();


         //add Jwt
         services.AddAuthentication()
            .AddCookie()
            .AddJwtBearer(cfg =>
            {
               cfg.TokenValidationParameters = new TokenValidationParameters
               {
                  ValidIssuer = _config["Tokens:Issuer"],
                  ValidAudience = _config["Tokens:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
               };
            });


         services.AddDbContext<DutchContext>(cfg =>
            {
               cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
            });

         services.AddAutoMapper();

         services.AddTransient<DutchSeeder>();

         services.AddTransient<IMailService, NullMailService>();
         // add support of real mail support

         services.AddScoped<IDutchRepository, DutchRepository>();

         services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      // Run this code every-time a request comes in
      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment() || env.IsStaging())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseExceptionHandler("/error");
         }

         //use default files changes the path from url to add default page links index.html, index.htm etc
         //app.UseDefaultFiles();
         app.UseStaticFiles();

         //Need to add Authentication right before UseMvc
         app.UseAuthentication();
         app.UseMvc(cfg =>
         {
            cfg.MapRoute("Default",
               "/{controller}/{action}/{id?}",
               new { controller = "App", Action = "Index" });
         });
         app.UseNodeModules(env);
      }
   }
}
