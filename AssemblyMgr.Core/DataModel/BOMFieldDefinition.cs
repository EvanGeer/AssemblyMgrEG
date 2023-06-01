using AssemblyMgrShared.Extensions;
using System;
using System.Collections.Generic;

namespace AssemblyMgrShared.DataModel
{
    public class BOMFieldDefinition : IStringSearchable
    {
        public string ParameterName { get; set; }
        public string ColumnHeader { get; set; }
        public double ColumnWidth { get; set; }
        public int Id { get; set; }

        public BOMFieldDefinition(string parameterName) : this(parameterName, parameterName, 0.5) { }
        public BOMFieldDefinition(string parameterName, string header, double width)
        {
            ParameterName = parameterName;
            ColumnHeader = header;
            ColumnWidth = width;
        }

        public override bool Equals(object obj)
        {
            return obj is BOMFieldDefinition definition &&
                   ParameterName == definition.ParameterName;
        }

        public override int GetHashCode()
        {
            return 908318635 + EqualityComparer<string>.Default.GetHashCode(ParameterName);
        }

        public bool PassesSearch(string queryString)
        {
            return string.IsNullOrEmpty(queryString)
                || ParameterName.Contains(queryString, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString() => ParameterName;

        //public static implicit operator BOMFieldDefinition((string paramName, string header, double width) tuple) 
        //    => new BOMFieldDefinition(tuple.paramName, tuple.header, tuple.width);

        public static implicit operator BOMFieldDefinition(string paramName)
            => new BOMFieldDefinition(paramName);
    }
}
