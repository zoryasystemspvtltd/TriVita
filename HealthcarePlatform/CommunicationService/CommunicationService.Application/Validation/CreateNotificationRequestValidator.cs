using CommunicationService.Contracts.Notifications;
using FluentValidation;

namespace CommunicationService.Application.Validation;

public sealed class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequestDto>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.EventType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PriorityReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Recipients).NotEmpty();
        RuleFor(x => x.Channels).NotEmpty();
        RuleForEach(x => x.Channels).ChildRules(ch =>
        {
            ch.RuleFor(c => c.ChannelTypeReferenceValueId).GreaterThan(0);
            ch.RuleFor(c => c.TemplateCode).NotEmpty().MaximumLength(100);
        });
    }
}
