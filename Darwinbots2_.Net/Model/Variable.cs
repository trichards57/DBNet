using System;

namespace DarwinBots.Model
{
    public readonly struct Variable
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

        public static bool operator !=(Variable left, Variable right)
        {
            return !(left == right);
        }

        public static bool operator ==(Variable left, Variable right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Variable var && Equals(var);
        }

        public bool Equals(Variable other)
        {
            return Functional == other.Functional && Informational == other.Informational && Name == other.Name && Synonym == other.Synonym && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Functional, Informational, Name, Synonym, Value);
        }
    }
}
