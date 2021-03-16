using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FirstCustomMiddleware
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
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Map
            app.Map("/products", appBuilder =>
            {

                //UseWhen
                app.UseWhen(context => context.Request.Query.ContainsKey("title"), builder =>
                {
                    builder.Run(async context =>
                    {
                        var title = context.Request.Query["title"];
                        await context.Response.WriteAsync($"title is: {title}");
                    });
                });

                //MapWhen
                app.MapWhen(context => context.Request.Query.ContainsKey("branch"), builder =>
                {
                    builder.Run(async context =>
                    {
                        var branch = context.Request.Query["branch"];
                        await context.Response.WriteAsync($"branch is: {branch}");
                    });
                });


                //localhost:8000/products/details
                appBuilder.Map("/Details", HandleProductDetails());

                appBuilder.Use(async (context, next) =>
                {
                    var name = context.Request.Query["name"];
                    if (!string.IsNullOrWhiteSpace(name))
                        context.Items.Add("name", name);

                    await next.Invoke();
                });

                appBuilder.Run(async context =>
                {
                    //context.Items.TryGetValue("name", out var name);
                    var name = context.Items["name"];
                    await context.Response.WriteAsync($"my name is: {name}");
                });
            });


            // Use
            app.Use(async (context, next) =>
            {
                context.Items.Add("name" , "Fazel");
                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                var id = context.Request.Query["id"] ;
                await next.Invoke();

                await context.Response.WriteAsync("This is a use Middleware");
            });

            


            // Run -> lastest middleware executeable
            app.Run(async context =>
            {
                // Context.Items.TryGetValue("name", out var name);
                var name = context.Items["name"];
                await context.Response.WriteAsync("Run Executed Successfully");
            });
            

            #region Default Middlewares
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });*/


            #endregion

        }

        private static Action<IApplicationBuilder> HandleProductDetails()
        {
            return builder =>
            {
                builder.Run(async context =>
                {
                    await context.Response.WriteAsync($"this is product details page");
                });
            };
        }
    }
}
