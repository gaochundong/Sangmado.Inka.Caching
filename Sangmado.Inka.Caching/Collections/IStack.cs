using System.Collections.Generic;

namespace Sangmado.Inka.Caching
{
    public interface IStack<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        void Push(T item);
        T Pop();
        T Peek();
        bool Contains(T item);
        void Clear();
    }
}
