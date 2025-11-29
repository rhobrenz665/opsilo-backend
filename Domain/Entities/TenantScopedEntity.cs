using System;

namespace Domain.Entities
{
    public abstract class TenantScopedEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
    }
}