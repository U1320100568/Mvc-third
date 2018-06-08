using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using IntelligenceCloud;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

[assembly: OwinStartup(typeof(IntelligenceCloud.Services.Startup))]

namespace IntelligenceCloud.Services
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }

    
    public class UploaderHub : Hub
    {
        //用以下方法取得UploaderHub Instance，其他類別也可以用以此呼叫
        static IHubContext HubContext =
            GlobalHost.ConnectionManager.GetHubContext<UploaderHub>();

        public static void UpdateProgress(string connId, string name, float percentage,
                                            string progress, string message = null)
        {   //呼叫javascript端的updateProgress函式
            HubContext.Clients.Client(connId).updateProgress(name, percentage, progress, message);
        }

    }
}