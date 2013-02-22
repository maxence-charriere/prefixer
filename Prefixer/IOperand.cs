using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prefixer
{
    public enum OperandType
    {
        Alphabetical,
        Numeric,
        Expression
    }

    public interface IOperand
    {
        // @Properties
        OperandType Type { get; set; }


        // @Member Functions
        IOperand Reduce();
    }
}
