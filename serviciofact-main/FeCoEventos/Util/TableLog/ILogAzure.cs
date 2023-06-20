using FeCoEventos.Models;
using System.Diagnostics;

namespace FeCoEventos.Util.TableLog
{
    public interface ILogAzure
    {
        void SetUp(CustomJwtTokenContext context, LogRequest logRequest);

        void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0);

        void SaveLog(int codigo, string comment, ref Stopwatch time, LevelMsn level = LevelMsn.Info, byte[] document = null);

        string GetSession();
    }
}
