using System;
using System.Xml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.SignalR;

namespace PmWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
            ReadAppConfig();
        }
        public void ReadAppConfig()
        {
            string filepach = AppContext.BaseDirectory;            
            XmlDocument document = new XmlDocument();
            document.Load(filepach + "appconfig.xml");
            XmlNode config = document.SelectSingleNode("AppConfig");
            XmlNodeList configList = config.ChildNodes;
            foreach (XmlNode item in configList)
            {
                if(item.Name == "Connection")
                {
                    string datasource = item.Attributes["datasource"].Value;
                    XmlNodeList connList = item.ChildNodes;
                    foreach (XmlNode connstr in connList)
                    {
                        if(connstr.Name.ToUpper() == "MODELER")
                        {
                            PmConnections.Modconnstr = "Data Source=" + datasource + ";Initial Catalog=" + connstr.InnerText + ";" + connstr.Attributes["security"].Value + ";User ID=" + connstr.Attributes["username"].Value + ";Password=" + connstr.Attributes["password"].Value + ";" + connstr.Attributes["muti"].Value;
                        }
                        if(connstr.Name.ToUpper() == "SCHEDULE")
                        {
                            PmConnections.Schconnstr = "Data Source=" + datasource + ";Initial Catalog=" + connstr.InnerText + ";" + connstr.Attributes["security"].Value + ";User ID=" + connstr.Attributes["username"].Value + ";Password=" + connstr.Attributes["password"].Value + ";" + connstr.Attributes["muti"].Value;
                        }
                        if (connstr.Name.ToUpper() == "PMCONTROL")
                        {
                            PmConnections.Ctrlconnstr = "Data Source=" + datasource + ";Initial Catalog=" + connstr.InnerText + ";" + connstr.Attributes["security"].Value + ";User ID=" + connstr.Attributes["username"].Value + ";Password=" + connstr.Attributes["password"].Value + ";" + connstr.Attributes["muti"].Value;
                        }                        
                    }
                }
                if(item.Name.ToUpper() == "LOGINSETTINGS")
                {
                    XmlNodeList settingsList = item.ChildNodes;
                    foreach (XmlNode settings in settingsList)
                    {
                        if(settings.Name.ToUpper() =="LOGINNAME")
                        {
                            PmSettings.LoginColName = settings.Attributes["name"].Value;
                        }
                        if(settings.Name.ToUpper() == "VIEWSYSCOL")
                        {
                            PmSettings.SysNameColName = settings.Attributes["name"].Value;
                        }
                    }    
                }
                if(item.Name.ToUpper() == "USERAPPCONFIG")
                {
                    XmlNodeList settingsList = item.ChildNodes;
                    foreach (XmlNode settings in settingsList)
                    {
                        if (settings.Name.ToUpper() == "VERSIONCODE")
                        {
                            PmSettings.AppVersion = settings.Attributes["name"].Value;
                        }                        
                        if(settings.Name.ToUpper() == "VERSIONGUID")
                        {
                            PmSettings.VersionGuid = settings.Attributes["name"].Value;
                        }
                        if(settings.Name.ToUpper() == "VERSIONMSG")
                        {
                            PmSettings.VersionMsg = settings.Attributes["name"].Value;
                        }
                    }
                }
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opertions =>
            {
                opertions.AddPolicy("any", bulider =>
                 {
                     bulider.SetIsOriginAllowed(op => true)
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
                     
                 });
            });
            services.AddSignalR();
            services.AddMvcCore().AddNewtonsoftJson(options => {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("any");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RateFactotyChatHub>("/chathub");
            });
        }

    }
}
