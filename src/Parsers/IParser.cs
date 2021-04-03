using LogParser.Models;
using System.Threading.Tasks;

namespace LogParser.Parsers
{
    public interface IParser
    {
        string ParserType { get; }

        Task<Log> GetData();

        void SelectFile(string file);
    }
}
