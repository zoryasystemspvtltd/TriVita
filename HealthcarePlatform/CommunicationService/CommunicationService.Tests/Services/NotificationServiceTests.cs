using AutoMapper;
using CommunicationService.Application.Mapping;
using CommunicationService.Application.Options;
using CommunicationService.Application.Services;
using CommunicationService.Application.Validation;
using CommunicationService.Contracts.Notifications;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CommunicationService.Tests.Services;

public sealed class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notifications = new();
    private readonly Mock<INotificationTemplateRepository> _templates = new();
    private readonly Mock<INotificationLogRepository> _logs = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly IMapper _mapper;
    private readonly CommunicationOptions _options;

    public NotificationServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<CommunicationMappingProfile>());
        _mapper = cfg.CreateMapper();

        _options = new CommunicationOptions
        {
            ReferenceValueIds = new ReferenceValueIdsOptions
            {
                NotificationStatusQueued = 2,
                ChannelDeliveryPending = 10,
                QueuePending = 20
            }
        };

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(9);
    }

    private NotificationService CreateSut() =>
        new(
            _notifications.Object,
            _templates.Object,
            _logs.Object,
            _mapper,
            _tenant.Object,
            Options.Create(_options),
            new CreateNotificationRequestValidator(),
            new SendTemplateNotificationRequestValidator());

    [Fact]
    public async Task CreateAsync_WhenFacilityMissing_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();

        var result = await sut.CreateAsync(new CreateNotificationRequestDto
        {
            EventType = "Test",
            PriorityReferenceValueId = 1,
            Recipients = new[] { new RecipientInputDto { RecipientTypeReferenceValueId = 1 } },
            Channels = new[] { new ChannelRequestDto { ChannelTypeReferenceValueId = 1, TemplateCode = "T" } }
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
    }

    [Fact]
    public async Task CreateAsync_WhenTemplateMissing_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(5L);
        _templates
            .Setup(t => t.GetByCodeAndChannelAsync(5L, "MISSING", 40, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ComNotificationTemplate?)null);

        var sut = CreateSut();
        var result = await sut.CreateAsync(new CreateNotificationRequestDto
        {
            EventType = "E",
            PriorityReferenceValueId = 1,
            Recipients = new[] { new RecipientInputDto { RecipientTypeReferenceValueId = 1, Email = "a@b.c" } },
            Channels = new[] { new ChannelRequestDto { ChannelTypeReferenceValueId = 40, TemplateCode = "MISSING" } }
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Template");
    }

    [Fact]
    public async Task CreateAsync_MultiRecipientAndChannel_Persists()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(5L);

        _templates
            .Setup(t => t.GetByCodeAndChannelAsync(5L, "T1", 40, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ComNotificationTemplate
            {
                Id = 500,
                TenantId = 1,
                FacilityId = 5,
                TemplateCode = "T1",
                TemplateName = "T",
                ChannelTypeReferenceValueId = 40,
                BodyTemplate = "Hi",
                Version = 1
            });

        ComNotification? captured = null;
        _notifications
            .Setup(n => n.AddAsync(It.IsAny<ComNotification>(), It.IsAny<CancellationToken>()))
            .Callback<ComNotification, CancellationToken>((e, _) => captured = e)
            .Returns(Task.CompletedTask);
        _notifications.Setup(n => n.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = CreateSut();
        var result = await sut.CreateAsync(new CreateNotificationRequestDto
        {
            EventType = "E",
            PriorityReferenceValueId = 1,
            Context = new Dictionary<string, string> { ["k"] = "v" },
            Recipients = new[]
            {
                new RecipientInputDto { RecipientTypeReferenceValueId = 1, Email = "a@test.com", IsPrimary = true },
                new RecipientInputDto { RecipientTypeReferenceValueId = 1, Email = "b@test.com" }
            },
            Channels = new[] { new ChannelRequestDto { ChannelTypeReferenceValueId = 40, TemplateCode = "T1" } }
        });

        result.Success.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Recipients.Should().HaveCount(2);
        captured.Channels.Should().HaveCount(1);
        captured.Queues.Should().HaveCount(1);
        captured.ContextJson.Should().Contain("k");
    }

    [Fact]
    public async Task GetTemplatesPagedAsync_ReturnsPaged()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(1L);
        _templates
            .Setup(t => t.GetPagedAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ComNotificationTemplate>(), 0));

        var sut = CreateSut();
        var result = await sut.GetTemplatesPagedAsync(new PagedQuery { Page = 1, PageSize = 20 });

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(0);
    }
}
