# SpeckyStandard

## Simplified injection:

To start Specky up just call the boot strapper at the beginning of your application.

        Specky.Manager.AutoStrapper.Start();

If you want to Speck multiple assemblies you will need to reference those assemblies in this call.

        SpeckyAutoStrapper.Start(new Assembly[]
        {
            typeof(Program).Assembly,
            typeof(Global.Models.Dtos.PersonDto).Assembly
        });

--------------------------------------------------------------------------------------------------------------------------

1. Add an interface.

        public interface ILogger
        {
            void Log(string msg);
        }

2. Use interface.
        
        public class Logger : ILogger
        {
            public void Log(string msg)
            {
                Console.WriteLine(msg);
            }
        }

3. Apply interface to any constructor.

        public class Worker
        {
            ILogger Logger { get; }

            public Worker(ILogger logger)
            {
                Logger = logger;
            }
        }

4. Now simply add the [Speck] attribute to the class you want to inject and to the class that will recieve the injection.

        [Speck]
        public class Logger : ILogger
        ...

        [Speck]
        public class Worker
        ...

--------------------------------------------------------------------------------------------------------------------------

Speck injection by configuration using Speck(configuration: "ConfigurationName").

Speck injection by name using SpeckName("NameofSpeck").

Speck injection using factories...

Make a SpeckyFactory by applying the [SpeckyFactory] attribute.
In the example below Specky calls GetMapper to inject the IMapper dynamically and injects the appropriate speck for EntityConfigurer automatically when calling the method.

        [SpeckyFactory]
        public class EntityMapFactory
        {
            [Speck]
            public IMapper GetMapper(EntityConfigurer entityConfigurer)
            {
                return new MapperConfiguration(entityConfigurer.ConfigureMappers).CreateMapper();
            }
        }

        [Speck]
        public class EntityConfigurer
        {
            public void ConfigureMappers(IMapperConfigurationExpression mapperConfigurationExpression)
            {
                CfgPersonDtoPerson(mapperConfigurationExpression);
            }

            private void CfgPersonDtoPerson(IMapperConfigurationExpression mapperConfigurationExpression)
            {
                mapperConfigurationExpression.CreateMap<PersonDto, Person>();
                mapperConfigurationExpression.CreateMap<Person, PersonDto>();
            }
        }

--------------------------------------------------------------------------------------------------------------------------

Make a custom conditional [SpeckAttribute] that only injects specks that past a given test. (Note: By convention we add the keyword Speck before any custom attributes.  This is not required but it will make your coworkers happy.)

1. Implement SpeckCondition into your own custom attribute.

        public class SpeckX64Attribute : SpeckyConditionAttribute
        {
            public override bool TestCondition()
            {
                return Environment.Is64BitProcess;
            }
        }

        public class SpeckX32Attribute : SpeckyConditionAttribute
        {
            public override bool TestCondition()
            {
                return !Environment.Is64BitProcess;
            }
        }

2. Apply the attribute to any type that you want injected only if that condition is met.

        [SpeckX64]
        public class LargeImageReader
        {
            ...
        }

        [SpeckX32]
        public class SmallImageReader
        {
            ...
        }


### *Specky is open source and free to use and distribute.*

> ### *I love, worship, and praise God with all my heart, might, and strength!* - **(Michael Brian Puckett, II)**