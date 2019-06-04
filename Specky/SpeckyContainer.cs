using Specky.DI;
using Specky.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Specky
{
    public sealed class SpeckyContainer
    {
        internal HashSet<InjectionModel> InjectionModels { get; } = new HashSet<InjectionModel>();

        SpeckyContainer() { }
        static public SpeckyContainer Instance { get; } = new SpeckyContainer();

        public void InjectSpeck(Type type, string speckName = "") => InjectSpeck(new InjectionModel(type, default, default, speckName));

        internal void InjectSpeck(InjectionModel injectionModel)
        {
            lock (this)
            {
                if (!string.IsNullOrWhiteSpace(injectionModel.SpeckName))
                {
                    var hasExistingSpeckName = InjectionModels.Any(x => x.SpeckName == injectionModel.SpeckName);
                    if (hasExistingSpeckName) throw new Exception($"A Speck already exists with a unique name of {injectionModel.SpeckName}\nOnly one unique name is allowed per Speck using the SpeckNameAttribute");
                }
                InjectionModels.Add(injectionModel);
            }
        }

        public T GetSpeck<T>() => (T)GetSpeck(typeof(T));

        public object GetSpeck(Type type)
        { 
            GetInstantiatedSpeck(type, out object speck);

            if (speck != null) return speck;

            GetExistingModel(type, out InjectionModel uninstantiatedModel);

            switch (uninstantiatedModel.DeliveryMode)
            {
                case DeliveryMode.PerRequest:
                    speck = SpeckActivator.InstantiateSpeck(type, uninstantiatedModel, HasSpeck, GetSpeck);
                    break;

                case DeliveryMode.SingleInstance:
                default:
                    speck = SpeckActivator.InstantiateSpeck(type, uninstantiatedModel, HasSpeck, GetSpeck);
                    var (speckType, deliveryMode, _, speckName) = uninstantiatedModel;
                    var instantiatedModel = new InjectionModel(speckType, deliveryMode, speck, speckName);
                    lock (this)
                    {
                        InjectionModels.Remove(uninstantiatedModel);
                        InjectionModels.Add(instantiatedModel);
                    }
                    break;
            }

            return speck;
        }

        public Task<object> GetSpeckAsync(Type type)
            => Task.Run(() => GetSpeck(type));

        private void GetInstantiatedSpeck(Type type, out object speck)
        {
            lock (this)
            {
                speck = InjectionModels
                       .Where(x => (x.Type == type) && (x.Instance != null) && (x.DeliveryMode == DeliveryMode.SingleInstance))
                       .Select(x => x.Instance)
                       .FirstOrDefault();
            }
        }

        public object GetSpeck(string typeName)
        {
            IReadOnlyList<InjectionModel> models;
            lock (this)
            {
                models = InjectionModels.Where(x => x.Type.Name == typeName || x.SpeckName == typeName).ToList();
            }
            if (models.Count > 1)
            {
                throw new Exception($"More than one Speck found with name of {typeName}.\nIt is recommended that all types have unique names.\nIf types must have the same name then define a unique name for each Speck with the SpeckName(speckName) attribute.");
            }
            var model = models.FirstOrDefault();
            if (model == null)
            {
                throw new Exception($"Specky has no registered types with name '{typeName}'.\nIf this is a unique name try using the SpeckName(speckName) attribute.");
            }
            return GetSpeck(model.Type);
        }

        public Task<object> GetSpeckAsync(string typeName)
            => Task.Run(() => GetSpeck(typeName));

        private void GetExistingModel(Type type, out InjectionModel model)
        {
            if (type.IsInterface)
            {
                lock (this)
                {
                    model = InjectionModels.FirstOrDefault(x => IsAssignable(type, x.Type))
                         ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }
            else
            {
                lock (this)
                {
                    model = InjectionModels.FirstOrDefault(x => x.Type == type)
                         ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }
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
