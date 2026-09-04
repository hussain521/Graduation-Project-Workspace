namespace Domain.Entities
{
    public class Role : BaseEntity
    {        
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
