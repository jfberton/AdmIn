namespace AdmIn.Data.Entidades
{
    public class PERMISO
    {
        public int PERM_ID { get; set; }

        private string _perm_nombre;
        public string PERM_NOMBRE
        {
            get => _perm_nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre del permiso no puede estar vacío.");
                _perm_nombre = value;
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(PERM_NOMBRE);
        }
    }
}
