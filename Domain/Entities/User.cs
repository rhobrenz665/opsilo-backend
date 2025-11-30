using Domain.Entities;

namespace Domain.Entities
{   
    public class User : TenantScopedEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public Guid? ProfileId { get; set; }
        public UserProfile? Profile { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
