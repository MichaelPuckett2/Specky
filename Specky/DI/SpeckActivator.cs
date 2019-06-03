using System;
using System.Linq;

namespace Specky.DI
{
    internal class SpeckActivator
    {
        internal static object InstantiateSpeck(Type requestedType, InjectionModel existingModel, Func<Type, bool> hasSpeck, Func<Type, object> getSpeck)
        {
            object instantiatedSpeck = null;
            Exception innerException = null;

            var isGenericTypeRequired = (requestedType != existingModel.Type) && (requestedType.IsGenericType && existingModel.Type.IsGenericType);

            if (isGenericTypeRequired) ReplaceWithGenericTypeModel(requestedType, ref existingModel);

            var constructors = existingModel
                .Type
                .GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count());

            foreach (var constructor in constructors)
            {
                var instantiatedParameters = constructor
                    .GetParameters()
                    .Where(p => hasSpeck(p.ParameterType))
                    .Select(p => getSpeck(p.ParameterType))
                    .ToArray();

                try
                {
                    instantiatedSpeck = Activator.CreateInstance(existingModel.Type, instantiatedParameters);
                }
                catch (Exception exception)
                {
                    innerException = new Exception(exception.Message, innerException);
                }
            }

            return instantiatedSpeck
                ?? throw new Exception($"Could not instantiate {existingModel.Type.Name}", innerException);
        }

        private static void ReplaceWithGenericTypeModel(Type requestedType, ref InjectionModel existingModel)
        {
            var typeArgs = requestedType.GetGenericArguments();
            var genericType = existingModel.Type.MakeGenericType(typeArgs);
            var (_, instance, deliveryMode) = existingModel;
            existingModel = new InjectionModel(genericType, instance, deliveryMode);
        }
    }
}
