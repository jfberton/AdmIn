using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Usuario
    {
        public Task<Usuario> Login(LoginModel model);

        Task<DTO<Usuario>> Actualizar_usuario(Usuario usuario);
        Task<DTO<Usuario>> Crear_usuario(Usuario usuario);
        Task<DTO<Usuario>> Obtener_usuario_por_email(string mail);
        Task<DTO<Usuario>> Obtener_usuario_por_email(string mail, string token);
        Task<DTO<bool>> Eliminar_usuario(int id);
        Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(Filtros_paginado filtros);
        Task<DTO<bool>> Modificar_password(CambioClaveModel datos);
    }
}
