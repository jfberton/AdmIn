using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using AdmIn.UI.Services.UtilityServices;

namespace AdmIn.UI.Services
{
 public class Serv_Reserva : IServ_ReservaUi
 {
 private readonly IHttpClientFactory _http;
 private readonly IConfiguration _config;
 private readonly ILogger<Serv_Reserva> _logger;
 private readonly ITokenService _tokenService;

 public Serv_Reserva(IHttpClientFactory http, IConfiguration config, ILogger<Serv_Reserva> logger, ITokenService tokenService)
 {
 _http = http; _config = config; _logger = logger; _tokenService = tokenService;
 }

 private HttpClient Client()
 {
 var c = _http.CreateClient();
 var token = _tokenService.GetTokenAsync().GetAwaiter().GetResult();
 if (!string.IsNullOrEmpty(token)) c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
 return c;
 }

 public async Task<DTO<Reserva>> Crear(ReservaCreateRequest req)
 {
 var client = Client();
 var resp = await client.PostAsJsonAsync("/api/reserva", req);
 if (!resp.IsSuccessStatusCode) return new DTO<Reserva> { Correcto = false, Mensaje = await resp.Content.ReadAsStringAsync() };
 var dto = await resp.Content.ReadFromJsonAsync<DTO<Reserva>>();
 return dto;
 }

 public async Task<DTO<Reserva>> Obtener_por_id(int id)
 {
 var client = Client();
 var resp = await client.GetAsync($"/api/reserva/{id}");
 if (!resp.IsSuccessStatusCode) return new DTO<Reserva> { Correcto = false, Mensaje = await resp.Content.ReadAsStringAsync() };
 var dto = await resp.Content.ReadFromJsonAsync<DTO<Reserva>>();
 return dto;
 }

 public async Task<DTO<IEnumerable<Reserva>>> Obtener_activas_por_inmueble(int inmuebleId)
 {
 var client = Client();
 var resp = await client.GetAsync($"/api/reserva/inmueble/{inmuebleId}");
 if (!resp.IsSuccessStatusCode) return new DTO<IEnumerable<Reserva>> { Correcto = false, Mensaje = await resp.Content.ReadAsStringAsync() };
 var dto = await resp.Content.ReadFromJsonAsync<DTO<IEnumerable<Reserva>>>();
 return dto;
 }

 public async Task<DTO<Reserva>> Actualizar(Reserva reserva)
 {
 var client = Client();
 var resp = await client.PutAsJsonAsync($"/api/reserva/{reserva.Id}", reserva);
 if (!resp.IsSuccessStatusCode) return new DTO<Reserva> { Correcto = false, Mensaje = await resp.Content.ReadAsStringAsync() };
 var dto = await resp.Content.ReadFromJsonAsync<DTO<Reserva>>();
 return dto;
 }
 }
}
