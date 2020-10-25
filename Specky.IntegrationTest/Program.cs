using Specky.Attributes;
using Specky.Enums;
using System;

namespace Specky.IntegrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Manager.Container
            //    .SetSpeck<Worker>()
            //    .AsSingleton()
            //    .As<IWorker>()
            //    .As<IWorker2>();

            Manager.AutoStrapper.Start();

            var worker = Manager.Container.Get<IWorker>();
            worker.DoWork();

            //var workerAgain = Manager.Container.Get<IWorker>();
            //workerAgain.DoWork();

            //Console.WriteLine(worker == workerAgain);

            var worker2 = Manager.Container.Get<IWorker2>();
            worker2.DoWork2();

            Console.WriteLine($"{ nameof(IWorker)} == {nameof(IWorker2)} : {worker == worker2}");

            Console.WriteLine("Hello World!");
        }
    }




    public interface IWorker
    {
        void DoWork();
    }

    public interface IWorker2
    {
        void DoWork2();
    }

    //[Speck]
    //[Speck(DeliveryMode.Singleton, new[] { typeof(IWorker), typeof(IWorker2) })]
    [Speck(DeliveryMode.Singleton, new[] { typeof(IWorker) })]
    [Speck(DeliveryMode.Singleton, new[] { typeof(IWorker2) })]
    public class Worker : IWorker, IWorker2
    {
        public void DoWork()
        {
            Console.WriteLine($"{nameof(Worker)} is working.");
        }

        public void DoWork2()
        {
            Console.WriteLine($"{nameof(Worker)} is working on 2.");
        }
    }
}
