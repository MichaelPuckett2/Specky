namespace Specky.IntegrationTest
{
    public class Worker : IWorker, IWorker2
    {
        public Worker(IWriter writer, IValidate<Person> validate)
        {
            Writer = writer;
            Validate = validate;
            Person = new Person
            {
                FirstName = "Michael",
                LastName = "Puckett",
                Age = 40
            };
        }

        public IWriter Writer { get; }
        public IValidate<Person> Validate { get; }
        public Person Person { get; }

        public void DoWork()
        {
            Writer.Write($"{nameof(Worker)} is working.");
            Writer.Write(Validate.Validate(Person).ToString());
        }

        public void DoWork2()
        {
            Writer.Write($"{nameof(Worker)} is working on 2.");
        }
    }
}
