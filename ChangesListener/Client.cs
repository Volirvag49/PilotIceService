/*
  Copyright © 2018 ASCON-Design Systems LLC. All rights reserved.
  This sample is licensed under the MIT License.
*/
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.Transport;

namespace ChangesListener
{
    class Client : IConnectionLostListener
    {
        private HttpPilotClient _client;
        private ConnectionCredentials _credentials;
        private IEventsApi _eventsApi;
        private List<MType> _types;

        public void StartListen(ConnectionCredentials credentials, List<DRule> rules)
        {
            _client?.Dispose();

            _credentials = credentials;
            _client = new HttpPilotClient(_credentials.GetConnectionString(), _credentials.GetConnectionProxy());
            _client.SetConnectionLostListener(this);
            _client.Connect(false);
            Console.WriteLine("Connected to database " + credentials.DatabaseName);

            _client.GetAuthenticationApi().Login(_credentials.DatabaseName,
                _credentials.Username,
                _credentials.ProtectedPassword,
                false,
                90);

            Console.WriteLine(_credentials.Username + " authenticated");

            var serverApi = _client.GetServerApi(null);
            serverApi.OpenDatabase();
            _types = serverApi.GetMetadata(0).Types;

            var callback = new EventsCallback(rules, AcceptChanges, PrintChangeDetails);
            _eventsApi = _client.GetEventsApi(callback);
            _eventsApi.SubscribeChanges(rules);
            Console.WriteLine("Subscribed " + rules.Count + " rule(s)");

            var missed = _eventsApi.GetMissedChanges();
            Console.WriteLine("Missed changes:");
            foreach (var tuple in missed)
            {
                var rule = rules.FirstOrDefault(x => x.Id == tuple.Item1);
                if (rule != null)
                {
                    Console.WriteLine($"Rule {rule.Id} {tuple.Item2.Length} changes:");
                    PrintChangeDetails(tuple.Item2, rule);

                    foreach (var dChangesetData in tuple.Item2)
                    {
                        AcceptChanges(dChangesetData.Identity, rule.Id);
                    }
                }
            }
        }

        private void AcceptChanges(Guid changesetId, Guid ruleId)
        {
            _eventsApi.AcceptChange(changesetId, ruleId);
        }


        private void PrintChangeDetails(IEnumerable<DChangesetData> changes, DRule rule)
        {
            foreach (var dChangesetData in changes)
            {
                foreach (var change in dChangesetData.Changes)
                {

                    var name = "";
                    if (change.New.Attributes.ContainsKey(SystemAttributes.PROJECT_ITEM_NAME))
                    {
                        name = change.New.Attributes[SystemAttributes.PROJECT_ITEM_NAME];
                        Console.WriteLine(GetRuleText(rule) + " файл " + name);
                    }
                    else
                    {
                        if (!change.New.ActualFileSnapshot.Files.Any())
                            continue;

                        var type = _types.First(x => x.Id == change.New.TypeId);
                        var attrs = type.Attributes.Where(x => x.ShowInTree).OrderBy(y => y.DisplaySortOrder);
                        foreach (var mAttribute in attrs)
                        {
                            DValue value;
                            change.New.Attributes.TryGetValue(mAttribute.Name, out value);
                            if (value != null)
                                name += " " + value.StrValue;
                        }

                        if (change.Old == null)
                            Console.WriteLine(GetRuleText(rule) + " документ " + name);
                        else
                            Console.WriteLine(GetRuleText(rule) + "а версия документа" + name);
                    }
                }
            }
        }

        private string GetRuleText(DRule rule)
        {
            switch (rule.ChangeType)
            {
                case ChangeType.Create:
                    return "Создан";
                case ChangeType.Delete:
                    return "Удален";
                case ChangeType.Update:
                    return "Изменен";
            }

            return "";
        }

        public void ConnectionLost(Exception ex = null)
        {
            Console.WriteLine("Connection lost");
            Console.WriteLine(ex);
        }
    }
}
