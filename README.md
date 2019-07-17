# SpeckyStandard

## Simplified injection:

1. Configure a Speck.

        [Speck]
        public class Log
        {
            public void Print(string message)
            {
                Console.WriteLine(message);
            }
        }

2. Inject a Speck.

        [Speck]
        public class Worker
        {
            readonly Log log;
            public Worker(Log log) => Log = log;
        }

--------------------------------------------------------------------------------------------------

### *Specky is open source and free to use and distribute.*

> ### *I love, worship, and praise God with all my heart, might, and strength!* - **(Michael Brian Puckett, II)**
