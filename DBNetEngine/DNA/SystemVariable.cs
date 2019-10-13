namespace DBNetEngine.DNA
{
    internal class SystemVariable
    {
        public SystemVariable(string name, int address, SystemVariableTypes type)
        {
            Name = name.ToLowerInvariant();
            Address = address;
            Type = type;
        }

        public int Address { get; }
        public string Name { get; }
        public SystemVariableTypes Type { get; }
    }
}