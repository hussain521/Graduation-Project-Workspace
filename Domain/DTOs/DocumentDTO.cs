using Domain.Entities;

namespace Domain.DTOs
{
    public class DocumentDTO
    {
        public Guid? Id { get; set; }       
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }        
        public int? SerialNum { get; set; }        
        public Guid? AccountId { get; set; }
        public int? TypeId { get; set; }
        public decimal? FinalTotal { get; set; }
        public string? Description { get; set; }
        public int? Item1Price { get; set; }
        public int? Item2Price { get; set; }
        public int? Item3Price { get; set; }
        public int? Item1Quantity { get; set; }
        public int? Item2Quantity { get; set; }
        public int? Item3Quantity { get; set; }
        public Guid? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
    }
}
