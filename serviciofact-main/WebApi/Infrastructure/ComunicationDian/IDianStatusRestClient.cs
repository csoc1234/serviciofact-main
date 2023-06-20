using Microsoft.Extensions.Configuration;
using System;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Infrastructure.ComunicationDian
{
    public interface IDianStatusRestClient
    {
        DianStatusResponse GetDianStatus(string cufe, ILogMongo log);

        DianStatusResponse GetStatusEvent(string cufe, ILogMongo log);
    }
}
