namespace ChinaImportPlatform.Api.Models;

/// <summary>A 1:1 conversation thread between a customer and the seller.</summary>
public class Conversation : BaseEntity
{
    public Guid CustomerId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public int UnreadForSeller { get; set; }

    public int UnreadForCustomer { get; set; }
}
