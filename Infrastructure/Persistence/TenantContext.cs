namespace Infrastructure.Persistence
{
    public class TenantContext
    {
        public Guid TenantId { get; set; } = Guid.Empty;
    }
}
