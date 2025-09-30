namespace AdmIn.Common.Entidades
{
    public class TrabajoAccionRequest
    {
        public int TrabajoId { get; set; }
        public int UsuarioId { get; set; }
        public string? Comentario { get; set; }
    }

    public class RevisarFinalizacionRequest
    {
        public int TrabajoId { get; set; }
        public int UsuarioId { get; set; }
        public bool Aprobado { get; set; }
        public string? Comentario { get; set; }
    }
}