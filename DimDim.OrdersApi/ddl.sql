CREATE TABLE ServiceOrders (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ServiceName NVARCHAR(120) NOT NULL,
    Area NVARCHAR(80) NOT NULL,
    TechnicianName NVARCHAR(120) NOT NULL,
    Status INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    TotalCost DECIMAL(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE ServiceParts (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PartName NVARCHAR(120) NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    ServiceOrderId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_ServiceParts_ServiceOrders
        FOREIGN KEY (ServiceOrderId) REFERENCES ServiceOrders(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ServiceOrders_Status ON ServiceOrders(Status);
CREATE INDEX IX_ServiceOrders_Area ON ServiceOrders(Area);
CREATE INDEX IX_ServiceOrders_Tech ON ServiceOrders(TechnicianName);
CREATE INDEX IX_ServiceParts_Order ON ServiceParts(ServiceOrderId);
