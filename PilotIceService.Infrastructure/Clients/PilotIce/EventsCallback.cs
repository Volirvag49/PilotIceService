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
        private readonly Func<Guid, Guid, Task>? _acceptAction;
        private readonly Func<IEnumerable<DChangesetData>, DRule, Task>? _processingChangeDetails;

        public EventsCallback(List<DRule> rules, Func<Guid, Guid, Task>? acceptAction,
            Func<IEnumerable<DChangesetData>, DRule, Task>? processingChangeDetails)
        {
            _rules = rules;
            _acceptAction = acceptAction;
            _processingChangeDetails = processingChangeDetails;
        }

        public async void NotifyChange(Guid ruleId, DChangesetData change)
        {
            var rule = _rules.FirstOrDefault(x => x.Id == ruleId);
            if (rule != null && _processingChangeDetails != null)
            {
                await _processingChangeDetails(new List<DChangesetData>() { change }, rule);
            }

            if (_acceptAction != null)
            {
                await _acceptAction(change.Identity, ruleId);
            }

        }

    }
}
