using System.Collections.Generic;
using WebApi.Domain.Models;
using WebApi.Models.Response;

namespace WebApi.Domain.Entity
{
    public class InvoiceEventsStatusDian : ResponseBase
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public InvoiceDocumentResponse Invoice { get; set; }
        public List<EventsDocumentResponse> Events { get; set; }
        public bool TieneEventos { get; set; }
        public int CantidadEventos { get; set; }
        public bool EsRecibida { get; set; }
        public bool EsReclamada { get; set; }
        public bool EsBienoServicio { get; set; }
        public bool EsAceptada { get; set; }
        public bool EsAceptadaTacitamente { get; set; }
        public bool EsTituloValor { get; set; }
        public bool EstaEndosada { get; set; }
    }
}
