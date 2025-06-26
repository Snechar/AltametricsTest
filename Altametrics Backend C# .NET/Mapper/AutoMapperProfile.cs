
using AutoMapper;
using Altametrics_Backend_C__.NET.Models.Entities;
using Altametrics_Backend_C__.NET.Models.DTOs.Auth;
using Altametrics_Backend_C__.NET.Models.DTOs.Event;
using Altametrics_Backend_C__.NET.Models.DTOs.RSVP;
using Altametrics_Backend_C__.NET.Models.DTOs.Audit;

namespace Altametrics_Backend_C__.NET.Mapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // User/AuthResponse
            CreateMap<User, AuthRespModel>();

            // Event
            CreateMap<CreateEventModel, Event>();
            CreateMap<EventUpdateModel, Event>();
            CreateMap<Event, EventRespModel>();

            // RSVP
            CreateMap<RSVPReqModel, RSVP>();
            CreateMap<RSVP, RSVPRespModel>()
                .ForMember(dest => dest.EventCode, opt => opt.MapFrom(src => src.EventCode))
                .ForSourceMember(src => src.EventId, opt => opt.DoNotValidate());

            // Audit
            CreateMap<AuditLog, AuditLogModel>();
        }
    }
}
