using System.Collections;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotIceService.Infrastructure.Kafka;
using PilotIceService.Infrastructure.WorkQueue;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public class PilotChangesListenerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IWorkQueue _workQueue;
        private readonly ILogger<PilotChangesListenerBackgroundService> _logger;

        public PilotChangesListenerBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration, IWorkQueue workQueue, ILogger<PilotChangesListenerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _workQueue = workQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
     
            var rules = LoadRules();


            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = CreateServiceScope();

                    var credentials = scope.ServiceProvider.GetRequiredService<ConnectionCredentials>();

                    var pilotClient =
                        new HttpPilotClient(credentials.GetConnectionString(), credentials.GetConnectionProxy());

                    using (await _workQueue.LockAsync(ct))
                    {
                        pilotClient.Connect(false);
                        pilotClient.GetAuthenticationApi()
                            .Login(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, false,
                                90);

                        var taskCompletionSource = new TaskCompletionSource<DSearchResult>();
                        var callBack = new SearchResultCallBack(taskCompletionSource.SetResult);
                        var serverApi = pilotClient.GetServerAsyncApi(callBack);

                        await serverApi.OpenDatabaseAsync();

                        //var metaData = await serverApi.GetMetadataAsync(0);

                        var eventsApi = pilotClient.GetEventsApi(new EventsCallback(rules,
                            null,
                            null));

                        await serverApi.OpenDatabaseAsync();
                        try
                        {
                            eventsApi.SubscribeChanges(rules);

                            // Получаем пропущенные изменения
                            var missed = eventsApi.GetMissedChanges();

                            foreach (var tuple in missed)
                            {
                                var rule = rules.FirstOrDefault(x => x.Id == tuple.Item1);
                                if (rule != null)
                                {
                                    _logger.LogInformation(
                                        $"Обнаружены пропущенные изменения: Rule {rule.Id} количество изменений: {tuple.Item2.Length}");
                                    await ProcessingChangesAsync(tuple.Item2, rule, ct);
                                    foreach (var dChangesetData in tuple.Item2)
                                    {
                                        await AcceptChangesAsync(eventsApi, dChangesetData.Identity, rule.Id, ct);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Ошибка при получении пропущенных изменений");
                        }

                        _logger.LogInformation($"Фоновый сервис ожидание: {DateTime.Now}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Фоновый сервис ожидание: {DateTime.Now}");
                }
            }
  
        }

        private List<DRule> LoadRules()
        {
            var rules = new List<DRule>
            {
                new DRule
                {
                    Id = new Guid("{853A5C30-5B36-4076-89F5-CA4764DEEB7F}"),
                    FileExtension = ".pdf",
                    ChangeType = ChangeType.Create
                },
                new DRule
                {
                    Id = new Guid("{0B0FE415-6FB3-4C5B-9301-2420477CBBFE}"),
                    FileExtension = ".xps",
                    ChangeType = ChangeType.Create
                },
                new DRule
                {
                    Id = new Guid("{C3E6454C-4901-44E9-BA67-8F861A06BB30}"),
                    FileExtension = ".txt",
                    ChangeType = ChangeType.Update
                },
                new DRule
                {
                    Id = new Guid("{395CE896-AC8C-4037-B99E-D759426621CF}"),
                    FileExtension = ".xps",
                    ChangeType = ChangeType.Delete
                }
            };

            // Можно добавить загрузку из конфигурации
            // var section = _configuration.GetSection("PilotRules");
            // section.Bind(rules);

            return rules;
        }

        private IServiceScope CreateServiceScope()
        {
            return _serviceProvider.CreateScope();
        }

        private async Task AcceptChangesAsync(IEventsApi eventsApi, Guid changesetId, Guid ruleId, CancellationToken ct)
        {
            try
            {
                eventsApi.AcceptChange(changesetId, ruleId);
                _logger.LogInformation($"Принято изменение: changesetId: {changesetId}, ruleId: {ruleId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при принятии изменения: changesetId: {changesetId}, ruleId: {ruleId}");
            }
        }

        private async Task ProcessingChangesAsync(IEnumerable<DChangesetData> changes, DRule rule, CancellationToken ct)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var queue = scope.ServiceProvider.GetRequiredService<IQueue>();
                var message = new PilotUpdatedMessage
                {
                    Changes = changes.ToList(),
                    Rule = rule
                };
                await queue.ProduceAsync("PilotIceService", message, ct);
                _logger.LogInformation($"Сообщение отправлено в Kafka для правила {rule.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при отправке сообщения в Kafka для правила {rule.Id}");
            }
        }
    }

    public class PilotUpdatedMessage
    {
        public List<DChangesetData> Changes { get; set; }
        public DRule Rule { get; set; }
    }
}