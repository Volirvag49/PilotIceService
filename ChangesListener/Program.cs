/*
  Copyright © 2018 ASCON-Design Systems LLC. All rights reserved.
  This sample is licensed under the MIT License.
*/
using Ascon.Pilot.Common.DataProtection;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChangesListener
{
    class Program
    {

        private static Guid ruleId1 = new Guid("019bea64-a312-7157-9707-872d71518283");
        private static Guid ruleId2 = new Guid("019bea64-a312-7157-9707-872d71518284");
        private static Guid ruleId3 = new Guid("019bea64-a312-7157-9707-872d71518285");





        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Регистрируем конфигурацию
                    services.AddSingleton<IConfiguration>(context.Configuration);

                    // Регистрируем клиент
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Build();



            var configuration = host.Services.GetRequiredService<IConfiguration>();

            var serverUrl = configuration.GetValue<string>("PilotConfig:ServerUrl");
            var userName = configuration.GetValue<string>("PilotConfig:UserName");
            var password = configuration.GetValue<string>("PilotConfig:Password");


            var credentials = ConnectionCredentials.GetConnectionCredentials(serverUrl, userName, password.ConvertToSecureString());
            var client = new Client();

            var rules = new List<DRule>();

            rules.Add(new DRule
            {
                Id = new Guid("{853A5C30-5B36-4076-89F5-CA4764DEEB7F}"),
                FileExtension = ".pdf",
                ChangeType = ChangeType.Create
            });

            rules.Add(new DRule
            {
                Id = new Guid("{0B0FE415-6FB3-4C5B-9301-2420477CBBFE}"),
                FileExtension = ".xps",
                ChangeType = ChangeType.Create
            });

            rules.Add(new DRule
            {
                Id = new Guid("{C3E6454C-4901-44E9-BA67-8F861A06BB30}"),
                FileExtension = ".txt",
                ChangeType = ChangeType.Update
            });

            rules.Add(new DRule
            {
                Id = new Guid("{395CE896-AC8C-4037-B99E-D759426621CF}"),
                FileExtension = ".xps",
                ChangeType = ChangeType.Delete
            });



            client.StartListen(credentials, rules);

            Console.ReadLine();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press eny key to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
