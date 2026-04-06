/*
  Copyright © 2018 ASCON-Design Systems LLC. All rights reserved.
  This sample is licensed under the MIT License.
*/
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api.Contracts;

namespace ChangesListener
{
    class EventsCallback : IEventsCallback
    {
        private readonly List<DRule> _rules;
        private readonly Action<Guid, Guid> _acceptAction;
        private readonly Action<IEnumerable<DChangesetData>, DRule> _printChangeDetails;

        public EventsCallback(List<DRule> rules, Action<Guid, Guid> acceptAction,
            Action<IEnumerable<DChangesetData>, DRule> printChangeDetails)
        {
            _rules = rules;
            _acceptAction = acceptAction;
            _printChangeDetails = printChangeDetails;
        }

        public void NotifyChange(Guid ruleId, DChangesetData change)
        {
            var rule = _rules.FirstOrDefault(x => x.Id == ruleId);
            if (rule != null)
            {
                _printChangeDetails(new List<DChangesetData>() { change }, rule);
            }

            _acceptAction(change.Identity, ruleId);
        }

    }
}