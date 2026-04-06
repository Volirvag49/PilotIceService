using Ascon.Pilot.ClientCore.Search;
using Ascon.Pilot.Common.Search;
using Ascon.Pilot.DataClasses;

namespace PilotIceService.Infrastructure.Clients.PilotIce
{
    public static class QueryBuilderExtensions
    {
        public static DSearchDefinition CreateSearchDefinition(this IQueryBuilder queryBuilder,
            int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct)
        {
            if (type != null)
            {
                queryBuilder.Must(ObjectFields.TypeId.BeAnyOf((int)type));
            }

            if (id.HasValue)
            {
                queryBuilder.Must(ObjectFields.Id.Be(id.Value));
            }

            if (parentId.HasValue)
            {
                queryBuilder.Must(ObjectFields.ParentId.Be(parentId.Value));
            }
            ////рутовые
            //else
            //{
            //    //test++++++++++++
            //    queryBuilder.Must(ObjectFields.ParentId.Be(DObject.RootId));
            //}

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                queryBuilder.Must(ObjectFields.AllText.Be($"*{searchString}*"));
            }

            return new DSearchDefinition
            {
                Id = Guid.NewGuid(),
                Request =
                {
                    MaxResults = maxResults ?? PilotClientConst.DefaultMaxResults,
                    SearchKind = SearchKind.Custom,
                    SearchString = queryBuilder.ToString(),
                },
            };
        }
    }
}