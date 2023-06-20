using APIAttachedDocument.Transversal.Model;
using System.Diagnostics;
using static APIAttachedDocument.Infrastructure.Logging.LogAzure;

namespace APIAttachedDocument.Infrastructure.Logging
{
    public interface ILogAzure
    {
        void SetUp(CustomJwtTokenContext context, string pnameMethod, IConfiguration configuration, ApplicationType application, string api);

        void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0);

        void SaveLog(int codigo, string comment, ref Stopwatch time, LevelMsn level = LevelMsn.Info, byte[] document = null);

        string GetSession();
    }
}
