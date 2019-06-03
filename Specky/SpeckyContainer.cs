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

        internal object GetSpeck(Type type)
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

        private bool CanTypeDeriveFromInterface(Type intr, Type type)
        {
            // EX:
            // intr = IInterface<object>
            // type = IntegerfaceImpl<T>
            var x = intr.ContainsGenericParameters; // false
            var y = type.ContainsGenericParameters; // true

            var xx = intr.IsConstructedGenericType; // true
            var yy = type.IsConstructedGenericType; // false

            var xxx = intr.IsGenericParameter; // false
            var yyy = type.IsGenericParameter; // false

            var xxxx = intr.IsGenericType; // true
            var yyyy = type.IsGenericType; // true

            var xxxxx = intr.IsGenericTypeDefinition; // false
            var yyyyy = type.IsGenericTypeDefinition; // true

            if (intr.IsGenericType)
            {
                Type[] typeArgumentsIntr = intr.GetGenericArguments();
                // object parameter
                // IsGenericParameter = false
                // IsGenericTypeParameter = false
                // IsTypeDefinition = true
            }

            if (type.IsGenericType)
            {
                Type[] typeArgumentsType = type.GetGenericArguments();
                // T parameter
                // IsGenericParameter = true
                // IsGenericTypeParameter = true
                // IsTypeDefinition = false

                var interfaces = type.GetInterfaces();
                foreach (var intrf in interfaces)
                {
                    if (intrf == intr)
                    {
                        var asdf = 10;
                    }
                }
            }

            return intr.IsAssignableFrom(type);
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
