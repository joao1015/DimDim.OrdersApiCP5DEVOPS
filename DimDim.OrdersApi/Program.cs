using DimDim.OrdersApi.Data;
using DimDim.OrdersApi.Dtos;
using DimDim.OrdersApi.Models;
using DimDim.OrdersApi.Observability;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB: SQL Server (Azure SQL em produção; local pode usar SQLEXPRESS ou Docker)
var cs = builder.Configuration.GetConnectionString("SqlServer")
         ?? Environment.GetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

// Telemetry (Application Insights via Azure Monitor OTEL)
builder.Services.AddDimDimTelemetry(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DimDim Orders API",
        Version = "v1",
        Description = "API de Ordens de Serviço (DimDim) com master–detail (ordem/peças)."
    });
});

var app = builder.Build();

// Migrar automaticamente (opcional; útil no App Service)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Listar ordens (pagina + filtros)
app.MapGet("/api/orders", async (AppDbContext db, int page = 1, int pageSize = 10, string? area = null, string? technician = null, OrderStatus? status = null) =>
{
    var q = db.ServiceOrders.AsNoTracking().Include(o => o.Parts).AsQueryable();
    if (!string.IsNullOrWhiteSpace(area)) q = q.Where(o => o.Area == area);
    if (!string.IsNullOrWhiteSpace(technician)) q = q.Where(o => o.TechnicianName == technician);
    if (status.HasValue) q = q.Where(o => o.Status == status);

    var total = await q.LongCountAsync();
    var items = await q.OrderByDescending(o => o.CreatedAtUtc)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .Select(o => new ServiceOrderOut(
                           o.Id, o.ServiceName, o.Area, o.TechnicianName,
                           o.Status, o.CreatedAtUtc, o.TotalCost,
                           o.Parts.Select(p => new ServicePartOut(p.Id, p.PartName, p.Quantity, p.UnitPrice, p.Quantity * p.UnitPrice)).ToList()
                       ))
                       .ToListAsync();

    return Results.Ok(new PagedResult<ServiceOrderOut>(items, page, pageSize, total));
})
.WithName("ListOrders")
.Produces<PagedResult<ServiceOrderOut>>(200);

// Buscar por Id
app.MapGet("/api/orders/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var o = await db.ServiceOrders.Include(x => x.Parts).FirstOrDefaultAsync(x => x.Id == id);
    if (o is null) return Results.NotFound();
    var dto = new ServiceOrderOut(
        o.Id, o.ServiceName, o.Area, o.TechnicianName, o.Status, o.CreatedAtUtc, o.TotalCost,
        o.Parts.Select(p => new ServicePartOut(p.Id, p.PartName, p.Quantity, p.UnitPrice, p.Quantity * p.UnitPrice)).ToList()
    );
    return Results.Ok(dto);
})
.WithName("GetOrder")
.Produces<ServiceOrderOut>(200).Produces(404);

// Criar
app.MapPost("/api/orders", async (AppDbContext db, ServiceOrderCreateIn body) =>
{
    var order = new ServiceOrder
    {
        ServiceName = body.ServiceName,
        Area = body.Area,
        TechnicianName = body.TechnicianName,
        Status = body.Status,
        Parts = body.Parts.Select(p => new ServicePart { PartName = p.PartName, Quantity = p.Quantity, UnitPrice = p.UnitPrice }).ToList()
    };
    db.ServiceOrders.Add(order);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetOrder", new { id = order.Id }, new { order.Id });
})
.WithName("CreateOrder")
.Produces(201);

// Atualizar (PUT substitui conteúdo)
app.MapPut("/api/orders/{id:guid}", async (AppDbContext db, Guid id, ServiceOrderUpdateIn body) =>
{
    var order = await db.ServiceOrders.Include(o => o.Parts).FirstOrDefaultAsync(o => o.Id == id);
    if (order is null) return Results.NotFound();

    order.ServiceName = body.ServiceName;
    order.Area = body.Area;
    order.TechnicianName = body.TechnicianName;
    order.Status = body.Status;

    // Resetar peças (simples) — poderia ser PATCH mais granular
    db.ServiceParts.RemoveRange(order.Parts);
    order.Parts = body.Parts.Select(p => new ServicePart { PartName = p.PartName, Quantity = p.Quantity, UnitPrice = p.UnitPrice }).ToList();

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateOrder")
.Produces(204).Produces(404);

// Excluir
app.MapDelete("/api/orders/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var order = await db.ServiceOrders.FindAsync(id);
    if (order is null) return Results.NotFound();
    db.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteOrder")
.Produces(204).Produces(404);

app.Run();
