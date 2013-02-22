using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prefixer
{
    public class Expression : IOperand
    {
        // @Delegate
        public delegate int CalcDelegate(int left, int right);

        // @Properties
        public IOperand Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }

        public IOperand Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }

        public char Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
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


        // @public
        public Expression()
        {
            _type = OperandType.Expression;
            _calcDictionary = new Dictionary<char, CalcDelegate>();
            _calcDictionary.Add('+', CalcAdd);
            _calcDictionary.Add('*', CalcMult);
            _calcDictionary.Add('/', CalcDiv);
            _calcDictionary.Add('%', CalcMod);
        }

        public IOperand Reduce()
        {
            _left = _left.Reduce();
            _right = _right.Reduce();
            if (_left.Type == OperandType.Numeric &&
                _right.Type == OperandType.Numeric)
            {
                int number = _calcDictionary[_operator](Convert.ToInt32(_left.ToString()),
                                                        Convert.ToInt32(_right.ToString()));
                return new Operand(number.ToString());
            }
            return this;
        }

        public override string ToString()
        {
            return ("(" + _operator + " " + _left.ToString() + " " + _right.ToString() + ")");
        }


        // @Private
        private IOperand _left;
        private IOperand _right;
        private char _operator;
        private OperandType _type;
        Dictionary<char, CalcDelegate> _calcDictionary;

        private bool IsAlphaOperand(char c)
        {
            return (('a' <= c && c <= 'z') ||
                    ('A' <= c && c <= 'Z'));
        }

        private int CalcAdd(int left, int right)
        {
            return (left + right);
        }

        private int CalcMult(int left, int right)
        {
            return (left * right);
        }

        private int CalcDiv(int left, int right)
        {
            return (left / right);
        }

        private int CalcMod(int left, int right)
        {
            return (left % right);
        }

    }
}
