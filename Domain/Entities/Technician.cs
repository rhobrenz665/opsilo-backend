namespace Domain.Entities
{
    public class Technician : TenantScopedEntity
    {
        public string Name { get; set; } = null!;
    }
}