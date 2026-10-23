/*
Datos de prueba
*/

USE Pedido;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM Ventas.Productos
)
BEGIN
    INSERT INTO Ventas.Productos
    (
        Nombre,
        Precio,
        Stock
    )
    VALUES
        (N'Laptop Lenovo ThinkPad', 45000.00, 10),
        (N'Mouse Logitech M185',      1500.00, 20),
        (N'Teclado Logitech K120',    2500.00, 5),
        (N'Monitor Dell 24"',        12500.00, 8),
        (N'Headset Logitech H390',    3500.00, 15),
        (N'Webcam Logitech C920',     5500.00, 3);
END
GO
