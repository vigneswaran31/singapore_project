using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpDeskEntity
{
    public enum WhereOperation
    {
        [StringValue("eq")]
        Equal,
        [StringValue("ne")]
        NotEqual,
        [StringValue("cn")]
        Contains,
        [StringValue("nc")]
        NotContains,
        [StringValue("gt")]
        GreaterThan,
        [StringValue("ge")]
        GreaterThanOrEqual,
        [StringValue("lt")]
        LessThan,
        [StringValue("le")]
        LessThanOrEqual,
        [StringValue("bw")]
        StartsWith,
        [StringValue("bn")]
        NotStartsWith,
        [StringValue("ew")]
        EndsWith,
        [StringValue("en")]
        NotEndsWith
    }
}
