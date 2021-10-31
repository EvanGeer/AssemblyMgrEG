using AssemblyMgrShared.Extensions;
using AssemblyMgrShared.UI;
using System;
using System.Collections.Generic;

namespace AssemblyManagerUI.DataModel
{
    public class BOMFieldDefinition : IStringSearchable
    {
        public string parameterName { get; set; }
        public string columnHeader { get; set; }
        public double columnWidth { get; set; }

        public BOMFieldDefinition(string ParameterName) : this(ParameterName, ParameterName, 0.5) { }
        public BOMFieldDefinition(string ParameterName, string Header, double Width)
        {
            parameterName = ParameterName;
            columnHeader = Header;
            columnWidth = Width;
        }

        public override bool Equals(object obj)
        {
            return obj is BOMFieldDefinition definition &&
                   parameterName == definition.parameterName;
        }

        public override int GetHashCode()
        {
            return 908318635 + EqualityComparer<string>.Default.GetHashCode(parameterName);
        }

        public bool PassesSearch(string queryString)
        {
            return string.IsNullOrEmpty(queryString)
                || parameterName.Contains(queryString, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString() => parameterName;

        public static implicit operator BOMFieldDefinition((string paramName, string header, double width) tuple) 
            => new BOMFieldDefinition(tuple.paramName, tuple.header, tuple.width);

        public static implicit operator BOMFieldDefinition(string paramName)
            => new BOMFieldDefinition(paramName);
    }
}
