using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LISService.Tests.Support;

/// <summary>
/// Shared setup for LIS entity CRUD controllers (all return HTTP 200 with <see cref="BaseResponse{T}"/> payloads).
/// </summary>
internal static class LisStandardCrudControllerTestTemplate
{
    public static ControllerContext DefaultContext => new() { HttpContext = new DefaultHttpContext() };

    public static Mock<ITenantContext> TenantMock(long tenantId = 1)
    {
        var t = new Mock<ITenantContext>();
        t.SetupGet(x => x.TenantId).Returns(tenantId);
        return t;
    }

    public static void AssertOkBaseResponse<T>(ActionResult<BaseResponse<T>> result, Action<BaseResponse<T>>? assertBody = null)
    {
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<T>>().Subject;
        assertBody?.Invoke(body);
    }

    public static void AssertOkPagedResponse<T>(ActionResult<BaseResponse<PagedResponse<T>>> result, Action<BaseResponse<PagedResponse<T>>>? assertBody = null)
    {
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<PagedResponse<T>>>().Subject;
        assertBody?.Invoke(body);
    }
}
