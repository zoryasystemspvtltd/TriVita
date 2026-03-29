using CommunicationService.Contracts.Notifications;
using FluentValidation;

namespace CommunicationService.Application.Validation;

public sealed class SendTemplateNotificationRequestValidator : AbstractValidator<SendTemplateNotificationRequestDto>
{
    public SendTemplateNotificationRequestValidator()
    {
        RuleFor(x => x.EventType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.TemplateCode).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ChannelTypeReferenceValueId).GreaterThan(0);
        RuleFor(x => x.PriorityReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Recipients).NotEmpty();
    }
}
