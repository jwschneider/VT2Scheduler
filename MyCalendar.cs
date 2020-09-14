using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace VT2Scheduler
{
    class MyCalendar
    {
        private readonly string ClientSecret = "9-_NMO1oEXU9cti28yV.mWDe188fY_S3V3";
        private readonly string ClientId = "f97b5d51-761c-49e1-9cce-9f1114fc76d8";
        private readonly string TenantId = "5f8386d8-442e-4ecd-9338-79cd77f4c139";

        private Calendar calendar;

        public MyCalendar()
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(ClientId)
            .WithTenantId(TenantId)
            .WithClientSecret(ClientSecret)
            .Build();
            
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var users = graphClient.Users.Request().GetAsync().Result;

            var me = users.CurrentPage[0];
            
        }
        public void AddEvent(ScheduledEvent scheduledEvent)
        {
            PrintEvent(scheduledEvent);
        }

        void PrintEvent(ScheduledEvent scheduledEvent)
        {
            Console.WriteLine($"Type: {scheduledEvent.Type}");
            Console.WriteLine($"Event: {scheduledEvent.Event}");
            Console.WriteLine($"Depart: {scheduledEvent.EDT}");
            Console.WriteLine($"Return: {scheduledEvent.RTB}");
            Console.WriteLine($"Instructor: {scheduledEvent.Instructor}");
            Console.WriteLine($"Location: {scheduledEvent.Location}");
        }
    }
}
