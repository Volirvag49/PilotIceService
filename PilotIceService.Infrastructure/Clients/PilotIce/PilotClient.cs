using Ascon.Pilot.ClientCore.Search;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.Transport;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public class PilotClient : IConnectionLostListener
    {
        private readonly ConnectionCredentials _credentials;

        public PilotClient(ConnectionCredentials connectionCredentials)
        {
            _credentials = connectionCredentials;
        }


        public async Task<MType[]?> GetTypesAsync(int? type, int? maxResults, CancellationToken ct)
        {

            //++++++++++++++
            //var serverApi2 = pilotClient.GetServerApi(callBack);

            //var backend = new Backend(serverApi2, default, default);
            //var root = backend.GetObject(DObject.RootId);

            //var types = backend.GetTypes();
            //var type2 = backend.GetType("Project");
            //var type3 = backend.GetType(19);
            //++++++++++++++

            var pilotClient =
                new HttpPilotClient(_credentials.GetConnectionString(), _credentials.GetConnectionProxy());

            pilotClient.Connect(false);
            pilotClient.GetAuthenticationApi()
                .Login(_credentials.DatabaseName, _credentials.Username, _credentials.ProtectedPassword, false, 90);


            var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
            var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
            var serverApi = pilotClient.GetServerAsyncApi(callBack);

            await serverApi.OpenDatabaseAsync();

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

        public async Task<IEnumerable<DObject>> SearchDObjects(int? type, int? maxResults, Guid? id, Guid? parentId,
            string? searchString, CancellationToken ct)
        {
            var searchDefinition = QueryBuilderFactory
                .CreateEmptyQueryBuilder()
                .CreateSearchDefinition(type, maxResults, id, parentId, searchString, ct);

            return await SearchEntities(searchDefinition, ct);
        }

        private async Task<IEnumerable<DObject>> SearchEntities(DSearchDefinition searchDefinition,
            CancellationToken ct)
        {
            var searchResult = await SearchObjects(searchDefinition, ct);
            if (searchResult?.Found == null)
            {
                return [];
            }


            var pilotClient =
                new HttpPilotClient(_credentials.GetConnectionString(), _credentials.GetConnectionProxy());

            pilotClient.Connect(false);
            pilotClient.GetAuthenticationApi()
                .Login(_credentials.DatabaseName, _credentials.Username, _credentials.ProtectedPassword, false, 90);


            var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
            var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
            var serverApi = pilotClient.GetServerAsyncApi(callBack);

            await serverApi.OpenDatabaseAsync();

            var result = await serverApi.GetObjectsAsync(searchResult?.Found.ToArray()).WaitAsync(ct);

            return result;
        }


        private async Task<DSearchResult> SearchObjects(
            DSearchDefinition searchDefinition, CancellationToken ct)
        {
            var taskCompletionSource =
                new TaskCompletionSource<DSearchResult>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Регистрируем отмену через токен
            await using var registration = ct.Register(() => taskCompletionSource.TrySetCanceled());

            var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);

            var pilotClient =
                new HttpPilotClient(_credentials.GetConnectionString(), _credentials.GetConnectionProxy());

            pilotClient.Connect(false);
            pilotClient.GetAuthenticationApi()
                .Login(_credentials.DatabaseName, _credentials.Username, _credentials.ProtectedPassword, false, 90);
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

        /// <inheritdoc />
        public void ConnectionLost(Exception ex = null)
        {
            throw new NotImplementedException();
        }
    }
}