using Specky.Attributes;
using Specky.Enums;
using System;

namespace Specky.IntegrationTest
{
    [Speck(DeliveryMode.Singleton)]
    public class ConsoleWriter : IWriter
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
