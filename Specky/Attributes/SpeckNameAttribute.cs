namespace Specky.Attributes
{
    public class SpeckNameAttribute : SpeckAttribute
    {
        public SpeckNameAttribute(string speckName)
        {
            SpeckName = speckName;
        }

        public string SpeckName { get; }
    }
}
