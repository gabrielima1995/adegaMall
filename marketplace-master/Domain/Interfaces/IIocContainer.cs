using System;

namespace Domain.Interfaces
{
    public interface IIocContainer
    {
        object Get(Type type);

        T Get<T>();
    }
}
