namespace DBNetModel.DNA
{
    public class SystemVariable
    {
        public SystemVariable(string name, int address, SystemVariableType type)
        {
            Name = name.ToLowerInvariant();
            Address = address;
            Type = type;
        }

        public int Address { get; }
        public string Name { get; }
        public SystemVariableType Type { get; }
    }
}