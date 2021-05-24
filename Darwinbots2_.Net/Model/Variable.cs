namespace Iersera.Model
{
    public class Variable
    {
        public Variable(string name, int value, bool informational = false, bool functional = false, string synonym = null)
        {
            Name = name;
            Value = value;
            Informational = informational;
            Functional = functional;
            Synonym = synonym;
        }

        public bool Functional { get; }
        public bool Informational { get; }
        public string Name { get; }
        public string Synonym { get; }
        public int Value { get; }
    }
}
