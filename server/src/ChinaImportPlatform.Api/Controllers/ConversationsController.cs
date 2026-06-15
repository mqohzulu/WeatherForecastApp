using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/conversations")]
[Produces("application/json")]
public class ConversationsController : ControllerBase
{
    private readonly IMessagingService _messaging;

    public ConversationsController(IMessagingService messaging)
    {
        _messaging = messaging;
    }

    /// <summary>The seller's unified inbox (US-S09).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConversationDto>>>> Get(CancellationToken ct)
    {
        var result = await _messaging.GetConversationsAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<ConversationDto>>.Ok(result));
    }

    /// <summary>Gets (or opens) the conversation for a customer (US-C17).</summary>
    [HttpGet("for-customer/{customerId:guid}")]
    public async Task<ActionResult<ApiResponse<ConversationDto>>> GetForCustomer(Guid customerId, CancellationToken ct)
    {
        var result = await _messaging.GetOrCreateForCustomerAsync(customerId, ct);
        return Ok(ApiResponse<ConversationDto>.Ok(result));
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MessageDto>>>> GetMessages(Guid id, CancellationToken ct)
    {
        var result = await _messaging.GetMessagesAsync(id, ct);
        return Ok(ApiResponse<IReadOnlyList<MessageDto>>.Ok(result));
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> Send(Guid id, [FromBody] SendMessageDto dto, CancellationToken ct)
    {
        // Ensure the route id is authoritative for the conversation.
        var message = await _messaging.SendAsync(dto with { ConversationId = id }, ct);
        return Ok(ApiResponse<MessageDto>.Ok(message));
    }
}
