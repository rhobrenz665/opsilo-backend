namespace Domain.Entities
{
  public class Ticket : TenantScopedEntity
    {
      public string Title { get; set; } = null!;
      public string Description { get; set; } = null!;
    }
}
