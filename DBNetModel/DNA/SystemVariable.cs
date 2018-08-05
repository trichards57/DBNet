using System;
using System.Collections.Generic;
using System.Text;

namespace DBNetModel.DNA
{
    [Flags]
    public enum SystemVariableType
    {
        Other = 0,
        Informational = 1,
        Functional = 2
    }

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