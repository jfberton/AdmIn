using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Utilidades
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CambioClaveModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NuevaPassword { get; set; }

    }
}
