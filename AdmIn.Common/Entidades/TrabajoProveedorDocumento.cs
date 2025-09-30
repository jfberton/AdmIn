using System;

namespace AdmIn.Common.Entidades
{
    public class TrabajoProveedorDocumento
    {
        public int TrabajoProveedor_DocumentoId { get; set; }
        public int TrabajoProveedorId { get; set; }
        public string NombreSubidor { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty; // factura, comprobante, etc.
        public DateTime FechaSubida { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        // Nota: no incluir aquí el arreglo de bytes para listados; se recupera sólo cuando se descarga
        public byte[]? Contenido { get; set; }
    }
}