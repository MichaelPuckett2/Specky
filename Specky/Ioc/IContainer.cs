using System;

namespace Specky.Ioc
{
    public interface IContainer
    {
        T Get<T>();
        Speck SetSpeck<T>();
        Speck SetSpeck(Type type);
    }
}
