using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.JobKits;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace CoreSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddHangfire(config =>
            {
                config.UseConsole();
                config.UseMemoryStorage();
                config.UseJobKits(typeof(Startup).Assembly);

            });
            services.AddMvc();

            var autofacContainer = services.BuildAutofacContainer();

            return autofacContainer.UseServiceProvider();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHangfireServer();

            app.UseHangfireDashboard("", new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>()
            });
            
        }
    }


    public static class AutofacConfig
    {
        /// <summary>
        /// 產生 Autofac 相依注入服務接口
        /// </summary>
        /// <param name="services">服務建置元件</param>
        /// <returns></returns>
        public static IContainer BuildAutofacContainer(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();

            builder.Populate(services);

            return builder.Build();
        }

        /// <summary>
        /// 取得相依注入服務接口
        /// </summary>
        /// <param name="container">Autofac Container</param>
        /// <returns></returns>
        public static IServiceProvider UseServiceProvider(this IContainer container)
        {
            return new AutofacServiceProvider(container);
        }
    }
    

}