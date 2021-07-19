using System;

namespace DarwinBots.Model
{
    public struct DnaBlock
    {
        public DnaBlock(int type, int value)
        {
            Type = type;
            Value = value;
        }

        public int Type { get; init; }
        public int Value { get; init; }

        public static bool operator !=(DnaBlock left, DnaBlock right)
        {
            return !(left == right);
        }

        public static bool operator ==(DnaBlock left, DnaBlock right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is DnaBlock block)
                return block.Type == Type && block.Value == Value;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}
