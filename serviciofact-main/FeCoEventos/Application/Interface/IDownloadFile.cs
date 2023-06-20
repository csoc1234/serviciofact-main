using FeCoEventos.Application.Dto;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IDownloadFile
    {
        FileDto GetFile(DownloadFileDto request, string tokenJwt, LogRequest logRequest);

        FileDto GetFileExternal(DownloadFileExternalDto request, string tokenJwt, LogRequest logRequest);
    }
}
