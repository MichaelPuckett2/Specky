using Specky.Enums;
using System;

namespace Specky.DI
{
    /// <summary>
    /// Immutable DVO representing Speck and injection.
    /// </summary>
    internal class InjectionModel
    {
        public InjectionModel(Type type) : this(type, default, default) { }
        public InjectionModel(Type type, DeliveryMode deliveryMode) : this(type, default, deliveryMode) { }
        public InjectionModel(Type type, object instance, DeliveryMode deliveryMode)
        {
            Type = type;
            Instance = instance;
            DeliveryMode = deliveryMode;
        }

        public void Deconstruct(out Type type, out object instance, out DeliveryMode deliveryMode)
        {
            type = Type;
            instance = Instance;
            deliveryMode = DeliveryMode;
        }

        public Type Type { get; }
        public object Instance { get; }
        public DeliveryMode DeliveryMode { get; }
    }
}