using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Microsoft.Owin;
using Owin;
using PlantillaMVC.App_Code;
using PlantillaMVC.Jobs.Jobs;

[assembly: OwinStartupAttribute(typeof(PlantillaMVC.Jobs.Startup))]
namespace PlantillaMVC.Jobs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            bool NotificationProcessEnabled = false;
            string JobName = string.Empty;
            string JobCron = string.Empty;
            string Dashboardurl = string.Empty;

            ConfigureAuth(app);

            try
            {
                //GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
                Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["Jobs.EnabledJobs"], out NotificationProcessEnabled);
                Dashboardurl = System.Configuration.ConfigurationManager.AppSettings["Jobs.Dashboard.Url"].ToString();

                if (NotificationProcessEnabled)
                {
                    JobName = System.Configuration.ConfigurationManager.AppSettings["Jobs.SincronizarDeals.Name"].ToString();
                    JobCron = System.Configuration.ConfigurationManager.AppSettings["Jobs.SincronizarDeals.Cron"].ToString();
                    RecurringJob.AddOrUpdate(JobName, () => DealsSyncJob.SyncDeals(), JobCron, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
                    //JobName = System.Configuration.ConfigurationManager.AppSettings["Jobs.SincronizarTickets.Name"].ToString();
                    //JobCron = System.Configuration.ConfigurationManager.AppSettings["Jobs.SincronizarTickets.Cron"].ToString();

                    //RecurringJob.AddOrUpdate(JobName, () => TicketsSyncJob.SyncTickets(), JobCron, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
                }

                app.UseHangfireDashboard(Dashboardurl, new DashboardOptions
                {
                    DisplayStorageConnectionString = false,
                    Authorization = new[] { new JobsAuthorizationFilter() },

                });
                app.UseHangfireServer();
            }
            catch
            {
                throw;
            }
        }
    }
}