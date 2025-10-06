using System.ComponentModel.DataAnnotations;
using DimDim.OrdersApi.Models;

namespace DimDim.OrdersApi.Dtos;

public record ServicePartIn(
    [property: Required, MaxLength(120)] string PartName,
    [property: Range(1, int.MaxValue)] int Quantity,
    decimal UnitPrice);

public record ServiceOrderCreateIn(
    [property: Required, MaxLength(120)] string ServiceName,
    [property: Required, MaxLength(80)] string Area,
    [property: Required, MaxLength(120)] string TechnicianName,
    OrderStatus Status,
    List<ServicePartIn> Parts);

public record ServiceOrderUpdateIn(
    [property: Required, MaxLength(120)] string ServiceName,
    [property: Required, MaxLength(80)] string Area,
    [property: Required, MaxLength(120)] string TechnicianName,
    OrderStatus Status,
    List<ServicePartIn> Parts);

public record ServicePartOut(Guid Id, string PartName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record ServiceOrderOut(
    Guid Id, string ServiceName, string Area, string TechnicianName,
    OrderStatus Status, DateTime CreatedAtUtc, decimal TotalCost, List<ServicePartOut> Parts);
