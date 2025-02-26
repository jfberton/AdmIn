using AdmIn.Business.Entidades;

namespace AdmIn.Mock.Services
{
    public interface IServ_Mock
    {
        Task<IEnumerable<Inmueble>> ObtenerInmuebles();
    }

    public class MockData : IServ_Mock
    {
        private static readonly Random random = new Random();

        public async Task<IEnumerable<Inmueble>> ObtenerInmuebles()
        {
            return await Task.Run(() =>
            {
                List<Inmueble> inmuebles = new();
                for (int i = 1; i <= 10; i++)
                {
                    inmuebles.Add(new Inmueble
                    {
                        Id = i,
                        Descripcion = $"Inmueble #{i}",
                        Comentario = "Hermoso inmueble en excelente ubicación.",
                        Valor = random.Next(50000, 500000),
                        Superficie = random.Next(50, 500),
                        Construido = random.Next(30, 450),
                        Telefono = new Telefono { Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}" },
                        Estado = new EstadoInmueble { Id = 1, Estado = (i % 2 == 0) ? "Disponible" : "Ocupado" },
                        ImagenUrl = $"https://picsum.photos/300/200?random={random.Next(1, 1000)}",
                        Direccion = new Direccion
                        {
                            DireccionId = i,
                            CalleNumero = $"Calle {random.Next(1, 200)} #{random.Next(1, 9999)}",
                            Colonia = "Centro",
                            Ciudad = "Ciudad Ejemplo",
                            Estado = "Estado Ejemplo",
                            CodigoPostal = "12345",
                            Pais = "Argentina"
                        }
                    });
                }
                return inmuebles;
            });
        }
    }

}
