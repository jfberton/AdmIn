using AdmIn.Business.Servicios;
using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IServ_Usuario _serv_usuario;

        public Auth(IConfiguration config, IServ_Usuario serv_usuario)
        {
            _config = config;
            _serv_usuario = serv_usuario;
        }


        [HttpPost("login")]
        public async Task<DTO<Usuario>> Login([FromBody] LoginModel user)
        {
            try
            {
                DTO<Usuario> respuesta = await _serv_usuario.ValidarCredenciales(user);

                if (respuesta.Correcto)
                {

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt:Key").Value);
                    var claims = new List<Claim>() { new Claim(ClaimTypes.Name, respuesta.Datos.Nombre) };

                    foreach (var permiso in respuesta.Datos.Permisos)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, permiso.Nombre));
                    }

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return new DTO<Usuario>()
                    {
                        Correcto = true,
                        Mensaje = "Usuario validado correctamente",
                        Datos = new Usuario
                        {
                            Id = respuesta.Datos.Id,
                            Nombre = respuesta.Datos.Nombre,
                            Email = respuesta.Datos.Email,
                            Roles = respuesta.Datos.Roles,
                            Permisos = respuesta.Datos.Permisos,
                            Token = tokenString
                        }
                    };
                }
                else
                {
                    return new DTO<Usuario>()
                    {
                        Correcto = true,
                        Mensaje = respuesta.Mensaje,
                        Datos = null
                    };
                }

            }
            catch (Exception ex)
            {
                return new DTO<Usuario>()
                {
                    Correcto = true,
                    Mensaje = $"Error al validar las credenciales. Error: {ex.Message}",
                    Datos = null
                };
            }
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetUser()
        {
            var claims = User.Identity as ClaimsIdentity;


            return Ok(claims.Name);
        }


        [HttpGet("protectedwithscope")]
        [Authorize(Roles = "user, mod, adm")]
        public IActionResult GetUserWithScope()
        {
            var claims = User.Identity as ClaimsIdentity;


            return Ok(claims.Name);
        }

        [HttpGet("protectedwithscope2")]
        [Authorize(Roles = "usuario, mod, admin")]
        public IActionResult GetUserWithScope2()
        {
            var claims = User.Identity as ClaimsIdentity;


            return Ok(claims.Name);
        }

    }
}
