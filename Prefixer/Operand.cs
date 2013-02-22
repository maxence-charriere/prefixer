using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prefixer
{
    public class Operand : IOperand
    {
        // @Properties
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public OperandType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }


        // @Public
        public Operand(string value)
        {
            if (value.Length == 0)
            {
                throw new ArgumentException("Error: Invalid argument passed in an Operand constructor.");
            }

            _value = value;
            if (('a' <= _value[0] && _value[0] <= 'z') ||
                ('A' <= _value[0] && _value[0] <= 'Z'))
            {
                _type = OperandType.Alphabetical;
            }
            else
            {
                _type = OperandType.Numeric;
            }
        }

        public IOperand Reduce()
        {
            return this;
        }

        public override string ToString()
        {
            return _value;
        }


        // @Private
        private string _value;
        private OperandType _type;
    }
}
