using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PmWebApi.Classes.StaticClasses;

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
            // "Mod": "Data Source=PMSER.SZRATETEC.COM,1436;Initial Catalog=MODELER_DB;Persist Security Info=True;User ID=sa ;Password=Szrate8520;MultiSubnetFailover=True;"
                     //Data Source = PMSER.SZRATETEC.COM,1436; Initial Catalog = MODELER_DB; Persist Security Info = True; User ID = sa; Password = System.Xml.XmlAttribute; MultiSubnetFailover = True
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
        }

    }
}
