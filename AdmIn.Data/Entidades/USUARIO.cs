namespace AdmIn.Data.Entidades
{
    public class USUARIO
    {
        public int USU_ID { get; set; }
        public string USU_NOMBRE { get; set; }
        public string USU_PASSWORD { get; set; }
        public string USU_EMAIL { get; set; }
        public DateTime USU_FECHA_CREACION { get; set; }
        public bool USU_ACTIVO { get; set; }

    }
}
