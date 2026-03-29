using AutoMapper;
using CommunicationService.Contracts.Notifications;
using CommunicationService.Domain.Entities;

namespace CommunicationService.Application.Mapping;

public sealed class CommunicationMappingProfile : Profile
{
    public CommunicationMappingProfile()
    {
        CreateMap<ComNotification, NotificationResponseDto>()
            .ForMember(d => d.RecipientCount, o => o.MapFrom(s => s.Recipients.Count))
            .ForMember(d => d.ChannelCount, o => o.MapFrom(s => s.Channels.Count))
            .ForMember(d => d.QueueCount, o => o.MapFrom(s => s.Queues.Count));

        CreateMap<ComNotificationLog, NotificationLogResponseDto>();

        CreateMap<ComNotificationTemplate, NotificationTemplateResponseDto>();
    }
}
