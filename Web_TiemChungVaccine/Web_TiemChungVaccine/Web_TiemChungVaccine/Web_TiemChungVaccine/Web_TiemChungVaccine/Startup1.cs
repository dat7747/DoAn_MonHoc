using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using System.Configuration;
using Hangfire;
using Hangfire.SqlServer;
using Web_TiemChungVaccine.Models;

[assembly: OwinStartup(typeof(Web_TiemChungVaccine.Startup1))]

namespace Web_TiemChungVaccine
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            HangfireJobs.LogMessage("Configuration method in Startup1 called.");

            string connectionString = ConfigurationManager.ConnectionStrings["TiemChungVCConnectionString1"].ConnectionString;

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            HangfireJobs.ScheduleJobs();
            HangfireJobs.LogMessage("Scheduled jobs.");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.UseHangfireServer();
        }
    }
}
