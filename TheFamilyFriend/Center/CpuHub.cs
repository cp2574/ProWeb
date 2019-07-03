using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TheFamilyFriend.Center
{
    [HubName("cpuHub")]
    public class CpuHub : Hub
    {
        public void Send(string meesges)
        {
            Clients.All.addNewMessageToPage(meesges);
        }
    }
}