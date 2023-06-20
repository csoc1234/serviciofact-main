using Contributors.Models;
using System.Diagnostics;
using static Contributors.Infraestructure.Logging.LogAzure;

namespace Contributors.Infraestructure.Logging.Interface
{
    public interface ILogAzure
    {
        void Setup(CustomJwtTokenContext context, string pnameMethod, ApplicationType application, string api);

        void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0);

        void SaveLog(int codigo, string comment, ref Stopwatch time, LevelMsn level = LevelMsn.Info, byte[] document = null);

        string GetSession();
    }
}
