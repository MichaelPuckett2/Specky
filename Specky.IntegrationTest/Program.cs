using Specky.Attributes;
using Specky.Enums;
using Specky.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Specky.IntegrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager.Container
                .SetSpeck<Worker>()
                .AsTransient()
                .As<IWorker>()
                .As<IWorker2>();

            Manager.AutoStrapper.Start();

            var worker = Manager.Container.Get<IWorker>();
            worker.DoWork();

            var worker2 = Manager.Container.Get<IWorker2>();
            worker2.DoWork2();

            Console.WriteLine($"{ nameof(IWorker)} == {nameof(IWorker2)} : {worker == worker2}");

            Console.WriteLine($"{ nameof(IWriter)} == {nameof(IWriter)} : {((Worker)worker).Writer == ((Worker)worker2).Writer}");

            Console.WriteLine("Hello World!");
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public interface IValidate<T>
    {
        bool Validate(T t);
    }

    [Speck]
    public class Greater10Validator : IValidate<Person>
    {
        public bool Validate(Person t) => t.Age > 18;
    }

    public class NameMichaelValidator : IValidate<Person>
    {
        public bool Validate(Person t) => t.FirstName != "Michael";
    }

    public class NameJohnValidator : IValidate<Person>
    {
        public bool Validate(Person t) => t.FirstName != "John";
    }

    [Speck]
    public class MichaelValidation : IValidate<Person>
    {
        public MichaelValidation(IEnumerable<IValidate<Person>> validators)
        {
            Validators = validators.ToList().AsReadOnly();
        }

        public IEnumerable<IValidate<Person>> Validators { get; }

        public bool Validate(Person t)
        {
            foreach (var validator in Validators)
            {
                if (!validator.Validate(t)) return false;
            }
            return true;
        }
    }

    public class JohnValidation : IValidate<Person>
    {
        public JohnValidation(IEnumerable<IValidate<Person>> validators)
        {
            Validators = validators.ToList().AsReadOnly();
        }

        public IEnumerable<IValidate<Person>> Validators { get; }

        public bool Validate(Person t)
        {
            foreach (var validator in Validators)
            {
                if (!validator.Validate(t)) return false;
            }
            return true;
        }
    }
}
