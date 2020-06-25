using Specky.DI;
using Specky.Enums;
using System;

namespace Specky.Dtos
{
    public class SpeckDto
    {
        public object Instance { get; }
        public bool IsSingleton { get; }
        public string Configuration { get; }
        public string SpeckName { get; set; }
        public Type Type { get; }
        public DeliveryMode DeliveryMode { get; }

        internal SpeckDto(InjectionModel  injectionModel)
        {
            Instance = injectionModel.Instance;
            IsSingleton = injectionModel.DeliveryMode != Enums.DeliveryMode.PerRequest
                       && injectionModel.DeliveryMode != Enums.DeliveryMode.Transient
                       && injectionModel.DeliveryMode != Enums.DeliveryMode.Scoped;
            Configuration = injectionModel.Configuration;
            SpeckName = injectionModel.SpeckName;
            Type = injectionModel.Type;
            DeliveryMode = injectionModel.DeliveryMode;
        }
    }
}
