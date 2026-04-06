using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    /// <summary>
    /// Old.
    /// </summary>
    public static class HttpPilotClientExtensions
    {
        public static void Connect(this HttpPilotClient pilotClient, ConnectionCredentials credentials)
        {
            pilotClient.Connect(false);
            pilotClient.GetAuthenticationApi()
                .Login(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, false, 90);
        }

        public static async Task<MType[]?> GetTypesAsync(this HttpPilotClient pilotClient, int? type, int? maxResults, CancellationToken ct)
        {
            var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
            var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
            var serverApi = pilotClient.GetServerAsyncApi(callBack);

            await serverApi.OpenDatabaseAsync().WaitAsync(ct);


            //++++++++++++++
            //var serverApi2 = pilotClient.GetServerApi(callBack);

            //var backend = new Backend(serverApi2, default, default);
            //var root = backend.GetObject(DObject.RootId);

            //var types = backend.GetTypes();
            //var type2 = backend.GetType("Project");
            //var type3 = backend.GetType(19);
            //++++++++++++++

            var metaData = await serverApi.GetMetadataAsync(0).WaitAsync(ct);

            var query = metaData?.Types.OrderBy(q => q.Id).AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(q => q.Id == type);
            }

            if (maxResults.HasValue)
            {
                query = query.Take(maxResults.Value);
            }
            else
            {
                query = query.Take(PilotClientConst.DefaultMaxResults);
            }

            return query.ToArray();
        }

        public static async Task<DSearchResult> SearchObjects(this HttpPilotClient pilotClient,
            DSearchDefinition searchDefinition, CancellationToken ct)
        {
            var taskCompletionSource =
                new TaskCompletionSource<DSearchResult>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Регистрируем отмену через токен
            await using var registration = ct.Register(() => taskCompletionSource.TrySetCanceled());

            var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
            var serverApi = pilotClient.GetServerAsyncApi(callBack);

            await serverApi.OpenDatabaseAsync().WaitAsync(ct);
            await serverApi.AddSearchAsync(searchDefinition).WaitAsync(ct);

            try
            {
                return await taskCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(PilotClientConst.DefaultRequestTimeOutSec), ct);
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<DObject>> GetObjects(this HttpPilotClient pilotClient, Guid[] ids,
            CancellationToken ct)
        {
            var serverApi = pilotClient.GetServerAsyncApi(new NullableServerCallback());
            await serverApi.OpenDatabaseAsync().WaitAsync(ct);
            var result = await serverApi.GetObjectsAsync(ids).WaitAsync(ct);
            return result;
        }
    }
}