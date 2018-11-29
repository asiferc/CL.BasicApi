using AutoMapper;

using Cardlytics.BasicApi.Models;
using Cardlytics.BasicApi.V1.Models;

namespace Cardlytics.BasicApi.Mapping
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            MapHealth();
        }

        private void MapHealth()
        {
            CreateMap<Health, HealthDto>()
                .ForMember(d => d.DataAccessHealthy, s => s.MapFrom(src => src.DatabaseConnectionVerified));
        }
    }
}
