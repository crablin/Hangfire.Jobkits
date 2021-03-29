using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreSample.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.JobKits;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHangfire(config =>
            {
                config.UseFilter<LogEverythingAttribute>(new LogEverythingAttribute());
                //   config.UseFilter(provider.GetRequiredService<AutomaticRetryAttribute>());
                config.UseConsole();
                //config.UseSqlServerStorage(@"Server=localhost\SQLEXPRESS,1433;Initial Catalog=HangFireDataBase;Persist Security Info=False;User ID=sa;Password=~1qaz;");
                //    config.UseMemoryStorage();
                config.UseSqlServerStorage(@"Server=54.249.186.202,1433;Initial Catalog=HangFireDataBase;Persist Security Info=False;User ID=sa;Password=#WSX2wsx!QAZ;");
                config.UseJobKits(typeof(Startup).Assembly);
                config.UseJobMonitor(typeof(Startup).Assembly);

            });
            services.AddJobMonitor();
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
            app.UseValidation(new JobValidation(app.ApplicationServices.GetService<IServiceProvider>()));
            app.UseHangfireMoitor(typeof(Startup).Assembly);
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