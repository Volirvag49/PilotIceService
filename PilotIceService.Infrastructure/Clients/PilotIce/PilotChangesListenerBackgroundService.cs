using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PilotIceService.Infrastructure.Kafka;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public class PilotChangesListenerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpPilotClient _client;
        private readonly IServerApi _serverApi;
        private readonly IEventsApi _eventsApi;
        private readonly IQueue _queue;
        private readonly ILogger<PilotChangesListenerBackgroundService> _logger;
        private readonly List<DRule> _rules = new List<DRule>()
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

        // EventsCallback создается и передается в GetEventsApi, но не используется напрямую

        /// <inheritdoc />
        public PilotChangesListenerBackgroundService(IServiceProvider serviceProvider, IQueue queue, ILogger<PilotChangesListenerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
            var scopeProvider = _serviceProvider.CreateScope();

            _client = scopeProvider.ServiceProvider
                .GetRequiredService<HttpPilotClient>();

            var credentials = scopeProvider.ServiceProvider
                .GetRequiredService<ConnectionCredentials>();

            _client.Connect(false);
            _client.GetAuthenticationApi()
                .Login(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, false, 90);

            var callback = new EventsCallback(_rules, AcceptChanges, PrintChangeDetailsAsync);
            _eventsApi = _client.GetEventsApi(callback);

            _serverApi = _client.GetServerApi(null);

        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _serverApi.OpenDatabase();
            var metaData = _serverApi.GetMetadata(0);

            try
            {
                // Получаем пропущенные изменения каждые 10 секунд
                var missed = _eventsApi.GetMissedChanges();

                foreach (var tuple in missed)
                {
                    var rule = _rules.FirstOrDefault(x => x.Id == tuple.Item1);
                    if (rule != null)
                    {
                        _logger.LogInformation($"Обнаружены пропущенные изменения: Rule {rule.Id} количество изменений: {tuple.Item2.Length}");
                        await PrintChangeDetailsAsync(tuple.Item2, rule);
                        foreach (var dChangesetData in tuple.Item2)
                        {
                            await AcceptChanges(dChangesetData.Identity, rule.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении изменений");
            }

            _eventsApi.SubscribeChanges(_rules);

            _logger.LogInformation("Начало прослушивания изменений...");

            while (!ct.IsCancellationRequested)
            {
                _logger.LogInformation($"Фоновый сервис Активен: {DateTime.Now}");
                await Task.Delay(10000, ct);
            }
        }


        private async Task AcceptChanges(Guid changesetId, Guid ruleId)
        {
            try
            {
                _eventsApi.AcceptChange(changesetId, ruleId);
                _logger.LogInformation($"Принято изменение: changesetId: {changesetId}, ruleId: {ruleId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при принятии изменения: changesetId: {changesetId}, ruleId: {ruleId}");
            }
        }


        private async Task PrintChangeDetailsAsync(IEnumerable<DChangesetData> changes, DRule rule)
        {
            try
            {
                var message = new PilotUpdatedMessage
                {
                    Changes = changes.ToList(),
                    Rule = rule
                };
                await _queue.ProduceAsync("PilotIceService", message, CancellationToken.None);
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
