using Ascon.Pilot.Common.DataProtection;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPilotIceClient(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetValue<string>("PilotConfig:ServerUrl");
            var userName = configuration.GetValue<string>("PilotConfig:UserName");
            var password = configuration.GetValue<string>("PilotConfig:Password");

            services.AddSingleton<ConnectionCredentials>(q => ConnectionCredentials.GetConnectionCredentials(url,
                userName,
                password.ConvertToSecureString()));

            services.AddSingleton<HttpPilotClient>(q =>
            {
                var credentials = q.GetService<ConnectionCredentials>();

                var pilotClient =
                    new HttpPilotClient(credentials.GetConnectionString(), credentials.GetConnectionProxy());

                pilotClient.Connect(false);
                pilotClient.GetAuthenticationApi()
                    .Login(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, false, 90);

                return pilotClient;
            });

            services.AddSingleton<IServerAsyncApi>(q =>
            {
                var pilotClient = q.GetService<HttpPilotClient>();

                var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
                var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
                var serverApi = pilotClient.GetServerAsyncApi(callBack);

                serverApi.OpenDatabaseAsync();

                return serverApi;
            });

            services.AddSingleton<IBackend>(q =>
            {
                var pilotClient = q.GetService<HttpPilotClient>();

                var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
                var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
                var serverApi = pilotClient.GetServerApi(callBack);

                return new Backend(serverApi, default, default);
            });

            services.AddSingleton<PilotClient>();

            services.AddHostedService<PilotChangesListenerBackgroundService>();

            return services;
        }
    }
}
