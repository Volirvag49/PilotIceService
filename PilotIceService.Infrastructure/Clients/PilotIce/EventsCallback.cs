/*
  Copyright © 2018 ASCON-Design Systems LLC. All rights reserved.
  This sample is licensed under the MIT License.
*/

using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api.Contracts;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    class EventsCallback : IEventsCallback
    {
        private readonly List<DRule> _rules;
        private readonly Func<Guid, Guid, Task> _acceptAction;
        private readonly Func<IEnumerable<DChangesetData>, DRule, Task> _printChangeDetails;

        public EventsCallback(List<DRule> rules, Func<Guid, Guid, Task> acceptAction,
            Func<IEnumerable<DChangesetData>, DRule, Task> printChangeDetails)
        {
            _rules = rules;
            _acceptAction = acceptAction;
            _printChangeDetails = printChangeDetails;
        }

        public async void NotifyChange(Guid ruleId, DChangesetData change)
        {
            var rule = _rules.FirstOrDefault(x => x.Id == ruleId);
            if (rule != null)
            {
                await _printChangeDetails(new List<DChangesetData>() { change }, rule);
            }

            await _acceptAction(change.Identity, ruleId);
        }

    }
}
