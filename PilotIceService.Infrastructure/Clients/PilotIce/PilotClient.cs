using Ascon.Pilot.ClientCore.Search;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public class PilotClient
    {
        private readonly HttpPilotClient _client;
        private readonly IServerAsyncApi _serverAsyncApi;
        private readonly IBackend _backend;

        public PilotClient(HttpPilotClient client, IServerAsyncApi serverAsyncApi, IBackend backend)
        {
            _client = client;
            _serverAsyncApi = serverAsyncApi;
            _backend = backend;
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

            var metaData = await _serverAsyncApi.GetMetadataAsync(0).WaitAsync(ct);

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

            var result = await _serverAsyncApi.GetObjectsAsync(searchResult?.Found.ToArray()).WaitAsync(ct);

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
            var serverApi = _client.GetServerAsyncApi(callBack);

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
    }
}