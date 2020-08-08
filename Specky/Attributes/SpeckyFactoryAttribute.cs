using Specky.Enums;
using System;

namespace Specky.Attributes
{
    /// <summary>
    /// Added to a class that will be used to instantiate Specks.
    /// The class should have a single method with a return type.
    /// The return type will be the type of speck injected.
    /// The method may be parameterless or contain parameters of interface or types.
    /// Specky will auto fill the parameters with other specks when calling this method.
    /// 
    /// Example:  IMapWorker Get(IWorker worker, IMapper mapper) { }
    /// 
    /// Specky will call Get and pass in the IWorker and IMapper specks loaded.
    /// The method will execute, performing any logic you need done to properly initialize the IMapWorker speck, return the IMapWorker and inject it into SpeckyContainer.
    /// Then IMapWorker can be retrieved from Specky exactly the same as retrieving any other Specks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SpeckyFactoryAttribute : SpeckNameAttribute
    {
        /// <summary>
        /// The default contructor for SpeckyFactory.  If using this contructor then be sure to include a SpeckAttribute to the method used by the factory.  
        /// This is what Specky invokes to inject specific specks.
        /// </summary>
        public SpeckyFactoryAttribute() : base(default, Lifetime.SingleInstance, default) { }

        /// <summary>
        /// Allows for customizing the speck method to determine the type of speck that will be injected.
        /// Alternatively you can use SpeckAttribute (or any of it's descendants) to capture this method.
        /// </summary>
        /// <param name="factoryMethodName">The name of the SpeckyFactory method that is used to return the Speck.</param>
        /// <param name="speckName">The name of the Speck returned from the factory method if required.</param>
        /// <param name="deliveryMode">The lifetime or deliverymode of the speck that Specky returns when needed.</param>
        /// <param name="configuration">The configuration used when Specky returns the speck.</param>
        public SpeckyFactoryAttribute(string factoryMethodName, string speckName = "", Lifetime deliveryMode = Lifetime.SingleInstance, string configuration = "")
        : base(string.IsNullOrEmpty(speckName) ? default : speckName, deliveryMode, string.IsNullOrEmpty(configuration) ? default : configuration)
        {
            FactoryMethodName = factoryMethodName;
        }

        public string FactoryMethodName { get; }
    }
}
