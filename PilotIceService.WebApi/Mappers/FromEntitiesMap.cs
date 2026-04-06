using Mappify;

namespace PilotIceService.WebApi.Mappers
{
    public class FromEntitiesMap : BaseMappingProfile
    {
        public override void CreateMaps(IMappify mappify)
        {
            mappify.CreateMap<RequestStatus, RequestStatus2>(source => new RequestStatus2
            {
                Id = source.Id,

            });
        }
    }
}