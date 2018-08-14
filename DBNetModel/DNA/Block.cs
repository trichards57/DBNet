using JetBrains.Annotations;

namespace DBNetModel.DNA
{
    public class Block
    {
        public Block(BlockType type, int value)
        {
            Type = type;
            Value = value;
        }

        public BlockType Type { get; }
        public int Value { get; }
    }
}