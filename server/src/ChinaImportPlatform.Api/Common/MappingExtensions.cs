using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Common;

/// <summary>
/// Entity-to-DTO projections kept in one place so controllers and services never
/// leak persistence models over the wire.
/// </summary>
public static class MappingExtensions
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        Email = user.Email,
        Role = user.Role,
        City = user.City,
        Suburb = user.Suburb,
        PreferredCollectionPoint = user.PreferredCollectionPoint
    };

    public static CategoryDto ToDto(this Category category, int productCount = 0) => new()
    {
        Id = category.Id,
        Name = category.Name,
        SortOrder = category.SortOrder,
        ImageUrl = category.ImageUrl,
        IsActive = category.IsActive,
        ProductCount = productCount
    };

    public static ProductVariantDto ToDto(this ProductVariant variant) => new()
    {
        Id = variant.Id,
        Attributes = variant.Attributes,
        PriceAdjustmentCents = variant.PriceAdjustmentCents,
        IsActive = variant.IsActive
    };

    public static ProductImageDto ToDto(this ProductImage image) => new()
    {
        Id = image.Id,
        S3Key = image.S3Key,
        Url = image.Url,
        SortOrder = image.SortOrder
    };

    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        CategoryId = product.CategoryId,
        Name = product.Name,
        Description = product.Description,
        IndicativePriceCents = product.IndicativePriceCents,
        LeadTime = product.LeadTime,
        IsActive = product.IsActive,
        Variants = product.Variants.Select(v => v.ToDto()).ToList(),
        Images = product.Images.OrderBy(i => i.SortOrder).Select(i => i.ToDto()).ToList()
    };

    public static TripDto ToDto(this Trip trip) => new()
    {
        Id = trip.Id,
        Name = trip.Name,
        CutoffDate = trip.CutoffDate,
        DepartureDate = trip.DepartureDate,
        Status = trip.Status
    };

    public static OrderItemDto ToDto(this OrderItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        VariantId = item.VariantId,
        ProductNameSnapshot = item.ProductNameSnapshot,
        CustomDescription = item.CustomDescription,
        Quantity = item.Quantity,
        Note = item.Note,
        UnitPriceFinalCents = item.UnitPriceFinalCents,
        LineStatus = item.LineStatus
    };

    public static OrderStatusHistoryDto ToDto(this OrderStatusHistoryEntry entry) => new()
    {
        Status = entry.Status,
        Note = entry.Note,
        PhotoS3Key = entry.PhotoS3Key,
        CreatedAt = entry.CreatedAt
    };

    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        UserId = order.UserId,
        TripId = order.TripId,
        Status = order.Status,
        PaymentStatus = order.PaymentStatus,
        TotalIndicativeCents = order.TotalIndicativeCents,
        TotalFinalCents = order.TotalFinalCents,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => i.ToDto()).ToList(),
        StatusHistory = order.StatusHistory.OrderBy(h => h.CreatedAt).Select(h => h.ToDto()).ToList()
    };

    public static AnnouncementDto ToDto(this Announcement announcement) => new()
    {
        Id = announcement.Id,
        Title = announcement.Title,
        Body = announcement.Body,
        ImageS3Key = announcement.ImageS3Key,
        CutoffDate = announcement.CutoffDate,
        Segment = announcement.Segment,
        PublishedAt = announcement.PublishedAt
    };

    public static PaymentDto ToDto(this Payment payment) => new()
    {
        Id = payment.Id,
        AmountCents = payment.AmountCents,
        Method = payment.Method,
        ProofS3Key = payment.ProofS3Key,
        GatewayReference = payment.GatewayReference,
        ApprovedAt = payment.ApprovedAt,
        CreatedAt = payment.CreatedAt
    };

    public static PaymentRequestDto ToDto(this PaymentRequest request) => new()
    {
        Id = request.Id,
        OrderId = request.OrderId,
        AmountCents = request.AmountCents,
        DueDate = request.DueDate,
        Reference = request.Reference,
        Instructions = request.Instructions,
        Status = request.Status,
        Payments = request.Payments.Select(p => p.ToDto()).ToList()
    };

    public static ConversationDto ToDto(this Conversation conversation, string? customerName = null) => new()
    {
        Id = conversation.Id,
        CustomerId = conversation.CustomerId,
        CustomerName = customerName,
        LastMessageAt = conversation.LastMessageAt,
        UnreadForSeller = conversation.UnreadForSeller,
        UnreadForCustomer = conversation.UnreadForCustomer
    };

    public static MessageDto ToDto(this Message message) => new()
    {
        Id = message.Id,
        ConversationId = message.ConversationId,
        SenderId = message.SenderId,
        OrderId = message.OrderId,
        Body = message.Body,
        AttachmentS3Key = message.AttachmentS3Key,
        SentAt = message.SentAt,
        DeliveredAt = message.DeliveredAt,
        ReadAt = message.ReadAt
    };
}
