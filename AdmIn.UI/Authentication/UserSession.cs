namespace AdmIn.UI.Authentication
{
    public class UserSession
    {
        public required string Email { get; set; }
        public required string Nombre { get; set; }
        public required string Password { get; set; }
        public required string Token { get; set; }
        public required string Roles { get; set; } // Roles en formato "Role1, Role2, Role3"
    }
}
