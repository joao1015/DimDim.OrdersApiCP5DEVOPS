using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimDim.OrdersApi.Models;

public class ServicePart
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(120)]
    public string PartName { get; set; } = default!;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // FK
    public Guid ServiceOrderId { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }

    [NotMapped]
    public decimal LineTotal => Quantity * UnitPrice;
}
