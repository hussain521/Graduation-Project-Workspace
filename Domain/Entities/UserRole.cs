namespace Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public User? User { get; set; }
        public Organization? Organization { get; set; }

        public Guid RoleId { get; set; }
        public Role? Role { get; set; }

        public bool Value { get; set; } = false;
        public bool IsActive { get; set; } = false;
    }
}
