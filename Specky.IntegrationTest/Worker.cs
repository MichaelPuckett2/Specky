namespace Specky.IntegrationTest
{
    public class Worker : IWorker, IWorker2
    {
        public Worker(IWriter writer)
        {
            Writer = writer;
        }

        public IWriter Writer { get; }

        public void DoWork()
        {
            Writer.Write($"{nameof(Worker)} is working.");
        }

        public void DoWork2()
        {
            Writer.Write($"{nameof(Worker)} is working on 2.");
        }
    }
}
