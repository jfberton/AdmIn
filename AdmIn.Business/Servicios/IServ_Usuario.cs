using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;


namespace AdmIn.Business.Servicios
{
    public interface IServ_Usuario
    {
        public Task<DTO<Usuario>> ValidarCredenciales(LoginModel login);
        Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(Filtros_paginado filtros);
        Task<DTO<Usuario>> Obtener_usuario(int id);
        Task<DTO<Usuario>> Obtener_usuario_mail(string mail);
        Task<DTO<Usuario>> Crear_usuario(Usuario usuario);
        Task<DTO<Usuario>> Actualizar_usuario(Usuario usuario);
        Task<DTO<bool>> Eliminar_usuario(int id);
        Task<DTO<bool>> ModificarContraseña(CambioClaveModel datos);
    }
}
