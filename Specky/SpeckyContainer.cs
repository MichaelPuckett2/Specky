using Specky.DI;
using Specky.Enums;
using Specky.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Specky
{
    public sealed class SpeckyContainer
    {
        internal HashSet<InjectionModel> InjectionModels { get; } = new HashSet<InjectionModel>();

        SpeckyContainer() { }
        static public SpeckyContainer Instance { get; } = new SpeckyContainer();

        public void Inject(Type type) => Inject(new InjectionModel(type));

        internal void Inject(InjectionModel injectionModel) => InjectionModels.Add(injectionModel);

        public T GetSpeck<T>() => (T)GetSpeck(typeof(T));

        public object GetSpeck(Type type)
        {
            object result = InjectionModels
                .Where(x => (x.Type == type) && (x.Instance != null) && (x.DeliveryMode == DeliveryMode.SingleInstance))
                .FirstOrDefault();

            if (result != null) return result;

            InjectionModel existingModel;

            existingModel = GetExistingModel(type);

            switch (existingModel.DeliveryMode)
            {
                case DeliveryMode.PerRequest:
                    result = SpeckActivator.InstantiateSpeck(type, existingModel, HasSpeck, GetSpeck);
                    break;

                case DeliveryMode.SingleInstance:
                default:
                    var instantiatedSpeck = SpeckActivator.InstantiateSpeck(type, existingModel, HasSpeck, GetSpeck);
                    var newModel = new InjectionModel(existingModel.Type, instantiatedSpeck, DeliveryMode.SingleInstance);
                    lock (this)
                    {
                        InjectionModels.Remove(existingModel);
                        InjectionModels.Add(newModel);
                    }
                    result = newModel.Instance;
                    break;
            }

            return result;
        }

        public object GetSpeck(string typeName)
        {
            var model = InjectionModels.FirstOrDefault(x => x.Type.Name == typeName);
            if (model == null) throw new Exception($"Specky has no registered types with name {typeName}");
            return GetSpeck(model.Type);
        }

        private InjectionModel GetExistingModel(Type type)
        {
            InjectionModel existingModel;
            if (type.IsInterface)
            {
                lock (this)
                {
                    existingModel = InjectionModels.FirstOrDefault(x => IsAssignable(type, x.Type))
                                 ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }
            else
            {
                lock (this)
                {
                    existingModel = InjectionModels.FirstOrDefault(x => x.Type == type)
                             ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }

            return existingModel;
        }

        internal bool HasSpeck(Type parameterType)
        {
            lock (this)
            {
                return parameterType.IsInterface
                    ? InjectionModels.FirstOrDefault(x => IsAssignable(parameterType, x.Type)) != null
                    : InjectionModels.FirstOrDefault(x => x.Type == parameterType) != null;
            }
        }

        public static bool IsAssignable(Type requestedType, Type injectedType)
        {
            if (requestedType.IsAssignableFrom(injectedType))
            {
                return true;
            }

            if ((requestedType.IsGenericType && injectedType.IsGenericType) && (requestedType.Assembly == injectedType.Assembly))
            {
                var interfaces = injectedType.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (requestedType.Name == @interface.Name) return true;
                }
            }

            return false;
        }

        ~SpeckyContainer()
        {
            foreach (var injectionModel in InjectionModels)
                if (injectionModel.Instance != null && injectionModel.Instance is IDisposable disposable)
                    try { disposable?.Dispose(); }
                    catch { }
        }
    }
}
