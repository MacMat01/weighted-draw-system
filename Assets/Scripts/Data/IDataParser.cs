using System.Collections.Generic;
namespace Data
{
    public interface IDataParser
    {
        List<T> Parse<T>(string rawText) where T : new();
    }
}
