using Mappify;

namespace PilotIceService.Infrastructure.Mappers
{
    public class FromEntitiesMap : BaseMappingProfile
    {
        public override void CreateMaps(IMappify mappify)
        {
            mappify.CreateMap<TestItem, TestItem2>(source => new TestItem2
            {
                Id = source.Id,

            });
        }
    }
}