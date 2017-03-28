using System.Collections.Generic;

namespace Sangmado.Inka.Caching
{
    public interface ICacheFactory
    {
        ISet<T> CreateSetCache<T>(string name);
        IList<T> CreateListCache<T>(string name);
        IDictionary<K, V> CreateDictionaryCache<K, V>(string name);
    }
}
