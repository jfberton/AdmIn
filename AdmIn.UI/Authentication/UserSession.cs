namespace AdmIn.UI.Authentication
{
    public class UserSession
    {
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Roles { get; set; } // Roles en formato "Role1, Role2, Role3"
    }
}
