using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdmIn.Common
{
    public class Filtros_paginado
    {
        [JsonPropertyName("filter")]
        public string? Filter { get; set; }

        [JsonPropertyName("top")]
        public int Top { get; set; }

        [JsonPropertyName("skip")]
        public int Skip { get; set; }

        [JsonPropertyName("orderby")]
        public string? OrderBy { get; set; }

        [JsonPropertyName("entityname")]
        public string EntityName { get; set; }

        public string WhereClause()
        {
            //SI NO HAY FILTRO RETORNAMOS UNA CADENA VACIA
            if (string.IsNullOrEmpty(Filter))
            {
                return "";
            }

            if (string.IsNullOrEmpty(EntityName) || !EntityMappings.EntityFieldMappings.ContainsKey(EntityName))
            {
                throw new Exception($"La entidad {EntityName} no tiene un mapeo definido.");
            }

            //SI EL FILTRO ES PARA UN COMBOBOX LA CADENA DEL WHERE LA ARMO DENTRO DEL COMPONENTE POR LO QUE DEVUELVO LA CADENA SIN MODIFICAR, QUITANDO UNICAMENTE EL ENCABEZADO
            if (Filter.Contains("WHERECOMBO"))
            {
                return Filter.Replace("WHERECOMBO", "");
            }


            //SINO OCURRIO NINGUNA DE LAS ANTERIORES , PROCEDO A ARMAR LA CADENA DEL WHERE

            List<campo_filtro> filtros_busqueda = new List<campo_filtro>();

            if (!string.IsNullOrEmpty(Filter))
                filtros_busqueda = ObtenerFiltrosBusqueda(Filter);

            StringBuilder whereClause = new StringBuilder();

            foreach (campo_filtro filtro in filtros_busqueda)
            {
                // Verificamos si el campo es una propiedad de navegación
                if (filtro.campo.Contains("."))
                {
                    // Si es una propiedad de navegación, separamos en las partes
                    string[] partes = filtro.campo.Split('.');

                    string tabla = partes[partes.Length - 2]; // El nombre de la clase asociada
                    string propiedad = partes[partes.Length - 1]; // La propiedad dentro de la clase asociada

                    // Construimos la cláusula WHERE con la propiedad de navegación
                    whereClause.Append($"{tabla}.{propiedad} LIKE '%{filtro.valor}%' AND ");
                }
                else
                {
                    // Si no es una propiedad de navegación, simplemente agregamos el filtro
                    whereClause.Append($"{filtro.campo} LIKE '%{filtro.valor}%' AND ");
                }
            }

            // Eliminamos el último "AND" de la cadena
            if (whereClause.Length > 0)
            {
                whereClause.Length -= 5;
            }

            return whereClause.ToString();
        }


        record campo_filtro
        {
            public string campo { get; set; }
            public string valor { get; set; }
        }

        private List<campo_filtro> ObtenerFiltrosBusqueda(string filtro)
        {
            List<campo_filtro> resultados = new List<campo_filtro>();
            string[] filtros = filtro.Split("and");
            foreach (string _filtro in filtros)
            {
                string campo;
                string valor;
                // Verificamos si el campo es una propiedad de navegación
                if (_filtro.Split(")").Length == 7)
                {
                    campo = _filtro.Split("(")[3].Split(")")[0].Trim();
                    valor = _filtro.Split("\"")[3].Trim();
                }
                else
                {
                    campo = _filtro.Split("(")[1].Split("==")[0].Trim();
                    valor = _filtro.Split("(\"")[1].Replace("\")", "").Trim();
                }

                campo = EntityMappings.EntityFieldMappings[EntityName][campo];

                resultados.Add(new campo_filtro
                {
                    campo = campo,
                    valor = valor
                });
            }

            return resultados;
        }
    }

    public class Items_pagina<T>
    {
        [JsonPropertyName("items")]
        public ICollection<T> Items { get; set; }

        [JsonPropertyName("total_items")]
        public int Total_items { get; set; }

    }

    public static class EntityMappings
    {
        public static readonly Dictionary<string, Dictionary<string, string>> EntityFieldMappings = new()
        {
            {
                "Usuario", new Dictionary<string, string>
                {
                    { "Id", "USU_ID" },
                    { "Nombre", "USU_NOMBRE" },
                    { "Password", "USU_PASSWORD" },
                    { "Email", "USU_EMAIL" },
                    { "Activo", "USU_ACTIVO" },
                    { "Creacion", "USU_FECHA_CREACION" }
                }
            },
            {
                "Rol", new Dictionary<string, string>
                {
                    { "Id", "ROL_ID" },
                    { "Nombre", "ROL_NOMBRE" },
                    { "Descripcion", "ROL_DESCRIPCION" }
                }
            }
        };
    }

}