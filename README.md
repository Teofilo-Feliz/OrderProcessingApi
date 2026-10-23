# Order Processing API

Este proyecto es una API REST para procesar pedidos de un comercio electrónico con reserva de stock. Lo más importante del sistema es mantener la consistencia del inventario: si alguno de los productos del pedido no tiene stock suficiente, se revierte toda la transacción y no se guarda el pedido ni se descuenta nada del inventario.

## Tecnologías

Para el desarrollo utilicé .NET 8 con ASP.NET Core Web API, Entity Framework Core 8 como ORM, SQL Server como base de datos y Postman para documentar y probar los endpoints.

## Contenido del repositorio

En la raíz del repositorio se encuentra la solución OrderProcessingApi.slnx con los cuatro proyectos de la API. La carpeta DataBase contiene los scripts SQL para crear la base de datos e insertar los datos de prueba. La carpeta Collections OrderProcessingApi contiene la colección de Postman con las peticiones para probar todos los endpoints.

## Arquitectura

Elegí una arquitectura por capas basada en los principios de Clean Architecture. La idea es que las dependencias apunten hacia el dominio y que la lógica de negocio no dependa de Entity Framework ni de SQL Server. La solución está dividida en cuatro proyectos.

OrderProcessingApi.Domain es la capa del dominio. Aquí están las entidades Producto, Pedido y PedidoDetalle. No depende de ninguna otra capa.

OrderProcessingApi.Application contiene la lógica de negocio. En la carpeta DTOs están los objetos de entrada y salida de la API, de esta forma nunca se exponen las entidades directamente. En Interfaces están los contratos de los servicios (IOrder e IProduct), de los repositorios (IOrderRepository e IProductRepository) y de la unidad de trabajo (IUnitOfWork). En Services están los casos de uso (OrderService y ProductService). En Exceptions está la excepción personalizada InsufficientStockException, y en Helpers la clase Result que uso para devolver el resultado de cada operación. Esta capa solo depende de Domain.

OrderProcessingApi.Infrastructure se encarga del acceso a datos con Entity Framework Core. En Configuration está el OrderProcessingApiDbContext junto con la configuración de cada entidad (tablas, esquema, tipos de columnas, índices y llaves foráneas). En Repository están las implementaciones de los repositorios y de la unidad de trabajo. Depende de Application y Domain.

OrderProcessingApi.Api es el punto de entrada. Aquí están los controladores, el registro de dependencias en Program.cs y la configuración de la cadena de conexión. Los controladores no tienen lógica de negocio, solo reciben la petición, llaman al servicio y convierten el resultado en la respuesta HTTP correspondiente.

### Patrones utilizados

Repository: cada entidad tiene su repositorio donde se encapsulan las consultas y operaciones sobre la base de datos.

Unit of Work: agrupa los repositorios y se encarga de guardar los cambios con SaveChangesAsync y de manejar la transacción con IDbContextTransaction. Como todos los repositorios comparten la misma instancia del DbContext, todas las operaciones quedan dentro de la misma transacción.

Result: los servicios devuelven un Result<T> que indica si la operación fue exitosa o contiene la lista de errores. Así la capa de aplicación no depende de HTTP y el controlador decide qué código de respuesta devolver.

Inyección de dependencias: los servicios, repositorios y la unidad de trabajo se registran como Scoped en Program.cs.

## Base de datos

Los scripts están en la carpeta DataBase y se deben ejecutar en este orden:

1. Script Create DataBase.sql: crea la base de datos Pedido, el esquema Ventas, las tablas, las llaves foráneas, las restricciones y los índices.
2. InsertValues.sql: inserta los productos de prueba.

Los dos scripts se pueden ejecutar más de una vez sin que den error ni dupliquen datos. El script de datos solo inserta los productos cuando la tabla está vacía, por lo que si se quiere volver al stock inicial después de hacer pruebas hay que eliminar la base de datos Pedido y ejecutar los dos scripts de nuevo.

La base de datos tiene tres tablas dentro del esquema Ventas.

Productos guarda el Id (generado por SQL Server), el Nombre, el Precio y el Stock. El nombre es obligatorio, y el precio y el stock no pueden ser negativos.

Pedidos guarda el Id, la FechaPedido, el Total y el Estado. El total lo calcula el backend y el estado puede ser Pendiente, Procesado o Cancelado.

PedidoDetalles guarda el Id, el PedidoId, el ProductoId, la Cantidad y el PrecioUnitario. Tiene llaves foráneas hacia Pedidos y Productos. El precio unitario se guarda para conservar el precio que tenía el producto al momento de hacer el pedido.

También agregué algunas restricciones adicionales. La restricción CK_Productos_Stock impide que el stock quede en negativo, lo que funciona como una segunda protección contra la sobreventa. La restricción UQ_PedidoDetalles_Pedido_Producto evita que un mismo producto se repita dentro de un pedido.

## Cadena de conexión

La cadena de conexión está en el archivo OrderProcessingApi.Api/appsettings.Development.json con el nombre DefaultConnection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\MSSQLSERVER01;Database=Pedido;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
}
```

En Program.cs se lee esa cadena y se registra el DbContext:

```csharp
builder.Services.AddDbContext<OrderProcessingApiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

Para correr el proyecto en otro equipo solo hay que cambiar el valor de Server por el nombre de la instancia de SQL Server que se tenga instalada. Por ejemplo, si se usa autenticación de Windows con la instancia por defecto:

```
Server=localhost;Database=Pedido;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;
```

Y si se usa autenticación de SQL Server:

```
Server=localhost;Database=Pedido;User Id=usuario;Password=clave;Encrypt=True;TrustServerCertificate=True;
```

La cadena está en el archivo de Development porque ese es el entorno que tienen configurado los perfiles de launchSettings.json. Si se quiere ejecutar en otro entorno hay que agregarla en el appsettings correspondiente.

## Cómo ejecutar el proyecto

Se necesita tener instalado el SDK de .NET 8 y SQL Server.

1. Ejecutar los scripts de la carpeta DataBase en el orden indicado.
2. Cambiar la cadena de conexión si es necesario.
3. Restaurar los paquetes y ejecutar la API:

```
dotnet restore
dotnet run --project OrderProcessingApi.Api --launch-profile https
```

4. La API queda disponible en https://localhost:7176 y en http://localhost:5022.

También se puede ejecutar desde Visual Studio poniendo OrderProcessingApi.Api como proyecto de inicio.

## Pruebas con Postman

Los endpoints los documenté y probé con Postman. La colección está en el archivo Collections OrderProcessingApi/Collections OrderProcessingApi.json y se puede importar en Postman con la opción Import.

La colección está organizada en dos carpetas, Productos y Pedidos, y cada petición tiene una descripción y pruebas automáticas que validan el código de respuesta y el contenido. Se pueden ejecutar una por una o todas juntas con el Runner de Postman.

La colección usa tres variables. La variable base_url tiene la URL de la API y por defecto apunta a https://localhost:7176; si Postman da error con el certificado de desarrollo se puede cambiar a http://localhost:5022 o desactivar la verificación SSL en la configuración de Postman. Las variables productoId y pedidoId se llenan automáticamente cuando se crea un producto o un pedido, y se usan en las peticiones que consultan por Id.

Para ejecutar la colección completa hay que tener la API en ejecución y la base de datos con los datos de prueba. Cada vez que se ejecuta la petición de crear pedido se descuenta stock, así que después de varias ejecuciones esa petición empieza a responder 400 por stock insuficiente, que es el comportamiento esperado.

En las peticiones POST el body se envía en la pestaña Body, seleccionando la opción raw y el formato JSON. El body debe ser un objeto JSON; por ejemplo, en el caso de los pedidos la lista de productos va dentro de la propiedad productos y no se envía como un arreglo suelto.

Los casos que probé fueron:

1. Consultar todos los productos y un producto por su Id, incluyendo un Id que no existe para verificar el 404.
2. Crear un producto con datos válidos y con datos inválidos (nombre vacío, precio en cero y stock negativo) para verificar las validaciones.
3. Crear un pedido con stock suficiente y verificar que el total se calcula con el precio de la base de datos y que el stock se descuenta.
4. Crear un pedido donde un producto no tiene stock suficiente y verificar que se responde 400 con la lista de productos rechazados y que el stock de los demás productos no cambia.
5. Crear un pedido con un producto que no existe y otro con la lista de productos vacía.
6. Consultar todos los pedidos y un pedido por su Id con sus detalles, incluyendo un Id que no existe para verificar el 404.

## Endpoints

Las rutas de la API las definí en inglés, igual que el resto del código, por lo que el endpoint de creación de pedidos que pide el requerimiento como POST /api/pedidos corresponde a POST /api/orders, y el de productos corresponde a /api/products.

### Productos

GET /api/products devuelve todos los productos con su Id, Nombre, Precio y Stock. Si no hay productos devuelve una lista vacía.

GET /api/products/{id} devuelve un producto específico. Si no existe responde 404 Not Found.

POST /api/products crea un nuevo producto y responde 201 Created con el Id generado. Ejemplo del body:

```json
{
  "nombre": "Impresora HP",
  "precio": 8500.00,
  "stock": 4
}
```

Al crear un producto se valida que el nombre no esté vacío y no pase de 150 caracteres, que el precio sea mayor que cero y que el stock sea mayor o igual a cero. Si algo no cumple se responde 400 Bad Request con todos los errores encontrados. El Id no se recibe desde el cliente, lo genera SQL Server.

### Pedidos

GET /api/orders devuelve los pedidos registrados con su Id, FechaPedido, Total y Estado.

GET /api/orders/{id} devuelve un pedido con sus detalles. Si no existe responde 404 Not Found.

POST /api/orders crea un pedido y descuenta el stock de los productos. Ejemplo del body:

```json
{
  "productos": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 3, "cantidad": 1 }
  ]
}
```

Si el pedido se crea correctamente responde 201 Created:

```json
{
  "mensaje": "Pedido creado correctamente.",
  "pedidoId": 1
}
```

Si algún producto no tiene stock suficiente o no existe, responde 400 Bad Request con la lista de productos rechazados:

```json
{
  "mensaje": "No fue posible procesar el pedido.",
  "errores": [
    "Producto: Webcam Logitech C920. Stock disponible: 3. Cantidad solicitada: 10.",
    "El producto con ID 99 no existe."
  ]
}
```

Al consultar un pedido por su Id se devuelve la información del pedido junto con sus productos, la cantidad, el precio unitario y el subtotal de cada uno:

```json
{
  "id": 1,
  "fechaPedido": "2026-10-23T14:30:00",
  "estado": "Pendiente",
  "total": 92500.00,
  "productos": [
    {
      "productoId": 1,
      "producto": "Laptop Lenovo ThinkPad",
      "cantidad": 2,
      "precioUnitario": 45000.00,
      "subtotal": 90000.00
    },
    {
      "productoId": 3,
      "producto": "Teclado Logitech K120",
      "cantidad": 1,
      "precioUnitario": 2500.00,
      "subtotal": 2500.00
    }
  ]
}
```

## Cómo se procesa un pedido

La lógica para crear un pedido está en el método CreateOrderAsync de OrderService. El proceso es el siguiente:

1. Primero valido que el pedido tenga al menos un producto y que los Id y las cantidades sean mayores que cero.
2. Si en el JSON viene el mismo producto más de una vez, lo agrupo y sumo las cantidades.
3. Abro una transacción a través de la unidad de trabajo.
4. Consulto los productos solicitados y verifico que existan y que tengan stock suficiente. Si uno o más no cumplen, lanzo la excepción InsufficientStockException con la lista de todos los productos rechazados.
5. Calculo el total con el precio que está guardado en la tabla Productos. El precio nunca se toma del JSON que envía el cliente.
6. Descuento el stock de cada producto con una actualización condicional (UPDATE ... WHERE Stock >= cantidad). Si otra petición consumió el stock entre la consulta y la actualización, no se actualiza ninguna fila y se lanza la misma excepción. Con esto se evita vender más de lo que hay cuando llegan pedidos al mismo tiempo.
7. Registro el pedido con sus detalles y guardo los cambios con SaveChangesAsync.
8. Confirmo la transacción.

Si ocurre cualquier error durante el proceso se hace rollback de la transacción, así no queda guardado el pedido ni se modifica el inventario. La excepción de stock se convierte en un Result fallido y el controlador responde 400 Bad Request con la lista de productos rechazados.

Todo el flujo es asíncrono con async y await, incluyendo las consultas, las actualizaciones, el guardado y el manejo de la transacción.
