namespace AdmIn.UI.Authentication
{
    public class UserSession
    {
        // Identificador del usuario (se usará en claims)
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Nombre { get; set; }
        public required string Password { get; set; }
        public required string Token { get; set; }
        public required string Roles { get; set; } // Roles en formato "Role1, Role2, Role3"
    }
}
