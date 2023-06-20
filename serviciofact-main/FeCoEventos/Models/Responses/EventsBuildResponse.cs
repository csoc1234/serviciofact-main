using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeCoEventos.Models.Responses
{
    public class EventsBuildResponse
    {
		[SwaggerSchema("Codigo de resultado")]
		public int Code { get; set; }

		[SwaggerSchema("Mensaje de resultado")]
		public string Message { get; set; }

		[SwaggerSchema("Archivo de evento")]
		public string Xml { get; set; }

		[SwaggerSchema("CUDE de evento")]
		public string Uuid { get; set; }

		[SwaggerSchema("Numero de evento")]
		public string EventId { get; set; }

		[SwaggerSchema("Numero de seguimiento")]
		public string TrackId { get; set; }

		[SwaggerSchema("Numero de seguimiento de email")]
		public int EmailRequestId { get; set; }

		[SwaggerSchema("Ruta del xml Evento")]
		public string PathFile { get; set; }

		[SwaggerSchema("Nombre de Archivo del Evento")]
		public string NameFile { get; set; }

		[SwaggerSchema("Tamaño del Archivo de Evento")]
		public int Size { get; set; }

		[SwaggerSchema("Hash del Archivo de Evento")]
		public string EventHash { get; set; }
	}
}
