# SpeckyStandard

## Simplified injection:

1. Introduce a Speck.

        [Speck]
        public class Log
        {
            public void Print(string message)
            {
                Console.WriteLine(message);
            }
        }

2. Introduce a Speck to a Speck.

        [Speck]
        public class Worker
        {
            [SpeckAuto]
            readonly Log log;
        }

3. Start when introductions are complete.

        [SpeckAuto]
        readonly Log log;

        [SpeckPost]
        public void Start()
        {
            Log.Print("Working...");
            //...
        }


---------------------------------------------------------------------------------------------------

1. Add Configurations.

        [SpeckConfiguration("Test")]
        class TestConfiguration
        {
            readonly string Reason = "Testing";
            readonly List<string> Names = new List<string>()
            {
                "Mathew", "Mark", "Luke", "John"
            };
        }

2. Set Configurations.

        [SpeckConfigurationAuto]
        public List<string> Names { get; }

3. Choose Configuration on startup.

        SpeckAutoStrapper.Start(configuration: "Test");

--------------------------------------------------------------------------------------------------

### Simple examples... There's more and much more coming.

### *Specky is open source and free to use and distribute.*

> ### *I love, worship, and praise God with all my heart, might, and strength!* - **(Michael Brian Puckett, II)**
