using System.ComponentModel.DataAnnotations;

namespace DimDim.OrdersApi.Models;

public enum OrderStatus { Open = 0, InProgress = 1, Done = 2, Canceled = 3 }

public class ServiceOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(120)]
    public string ServiceName { get; set; } = default!;   // "serviço"

    [Required, MaxLength(80)]
    public string Area { get; set; } = default!;          // "área"

    [Required, MaxLength(120)]
    public string TechnicianName { get; set; } = default!; // "técnico"

    public OrderStatus Status { get; set; } = OrderStatus.Open;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public decimal TotalCost { get; set; } = 0m;

    public List<ServicePart> Parts { get; set; } = new();
}
