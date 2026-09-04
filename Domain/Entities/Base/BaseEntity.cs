
namespace Domain.Entities.Base
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }        

        //[ForeignKey(nameof(User))]
        public virtual Guid? UserId { get; set; }

        //public virtual User? User { get; set; }

        public virtual Guid? OrganizationId { get; set; }
        //public virtual Organization? Organization { get; set; }
    }
}