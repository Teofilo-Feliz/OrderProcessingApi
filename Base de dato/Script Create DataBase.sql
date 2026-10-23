USE master;
GO

IF DB_ID(N'OrderProcessingApiDB') IS NULL
BEGIN
    CREATE DATABASE OrderProcessingApiDB;
END
GO

USE OrderProcessingApiDB;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.schemas
    WHERE name = N'Ventas'
)
BEGIN
    EXEC(N'CREATE SCHEMA Ventas');
END
GO

IF OBJECT_ID(N'Ventas.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE Ventas.Productos
    (
        Id INT IDENTITY(1,1) NOT NULL,

        Nombre NVARCHAR(150) NOT NULL,

        Precio DECIMAL(18,2) NOT NULL,

        Stock INT NOT NULL,

        CONSTRAINT PK_Productos
            PRIMARY KEY CLUSTERED (Id),

        CONSTRAINT CK_Productos_Nombre
            CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0),

        CONSTRAINT CK_Productos_Precio
            CHECK (Precio >= 0),

        CONSTRAINT CK_Productos_Stock
            CHECK (Stock >= 0)
    );
END
GO

IF OBJECT_ID(N'Ventas.Pedidos', N'U') IS NULL
BEGIN
    CREATE TABLE Ventas.Pedidos
    (
        Id INT IDENTITY(1,1) NOT NULL,

        FechaPedido DATETIME2(0) NOT NULL
            CONSTRAINT DF_Pedidos_FechaPedido
            DEFAULT SYSDATETIME(),

        Total DECIMAL(18,2) NOT NULL,

        Estado VARCHAR(20) NOT NULL
            CONSTRAINT DF_Pedidos_Estado
            DEFAULT 'Pendiente',

        CONSTRAINT PK_Pedidos
            PRIMARY KEY CLUSTERED (Id),

        CONSTRAINT CK_Pedidos_Total
            CHECK (Total >= 0),

        CONSTRAINT CK_Pedidos_Estado
            CHECK
            (
                Estado IN
                (
                    'Pendiente',
                    'Procesado',
                    'Cancelado'
                )
            )
    );
END
GO

IF OBJECT_ID(N'Ventas.PedidoDetalles', N'U') IS NULL
BEGIN
    CREATE TABLE Ventas.PedidoDetalles
    (
        Id INT IDENTITY(1,1) NOT NULL,

        PedidoId INT NOT NULL,

        ProductoId INT NOT NULL,

        Cantidad INT NOT NULL,

        PrecioUnitario DECIMAL(18,2) NOT NULL,

        CONSTRAINT PK_PedidoDetalles
            PRIMARY KEY CLUSTERED (Id),

        CONSTRAINT FK_PedidoDetalles_Pedidos
            FOREIGN KEY (PedidoId)
            REFERENCES Ventas.Pedidos(Id),

        CONSTRAINT FK_PedidoDetalles_Productos
            FOREIGN KEY (ProductoId)
            REFERENCES Ventas.Productos(Id),

        CONSTRAINT CK_PedidoDetalles_Cantidad
            CHECK (Cantidad > 0),

        CONSTRAINT CK_PedidoDetalles_PrecioUnitario
            CHECK (PrecioUnitario >= 0),

        CONSTRAINT UQ_PedidoDetalles_Pedido_Producto
            UNIQUE NONCLUSTERED
            (
                PedidoId,
                ProductoId
            )
    );
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PedidoDetalles_ProductoId'
      AND object_id = OBJECT_ID(N'Ventas.PedidoDetalles')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_PedidoDetalles_ProductoId
        ON Ventas.PedidoDetalles
        (
            ProductoId
        );
END
GO
