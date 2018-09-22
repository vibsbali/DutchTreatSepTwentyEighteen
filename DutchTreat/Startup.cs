using DutchTreat.Data;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
         services.AddDbContext<DutchContext>(cfg =>
            {
               cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
            });

         services.AddTransient<DutchSeeder>();

         services.AddTransient<IMailService, NullMailService>();
         // add support of real mail support

         services.AddScoped<IDutchRepository, DutchRepository>();

         services.AddMvc();
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
