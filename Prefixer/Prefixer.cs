using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prefixer
{
    public class Prefixer
    {
        // @Enumerations
        enum Tokens
        {
            Empty,
            WhiteSpace,
            Operand,
            Operator,
            LeftParenthesis
        }


        // @Porperties
        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                Init();
            }
        }


        // @Public
        public Prefixer(string input)
        {
            Input = input;
        }

        public string Parse()
        {
            return DoParse().ToString();
        }

        public string Reduce()
        {
            var expr = DoParse();
            expr = expr.Reduce();
            return expr.ToString();
        }


        // @Private
        private string _input;
        private int _idx;
        private string _currentOperandToken;
        private Tokens _lastToken;
        private Stack<Operand> _operandStack;
        private Stack<IOperand> _expressionStack;
        private Stack<char> _stack;

        private void Init()
        {
            _idx = 0;
            _lastToken = Tokens.Empty;
            _currentOperandToken = "";
            _operandStack = new Stack<Operand>();
            _expressionStack = new Stack<IOperand>();
            _stack = new Stack<char>();
        }

        private IOperand DoParse()
        {
            while (_idx < _input.Length)
            {
                if (IsWhiteSpace(_input[_idx]))
                {
                    WhiteSpaceWork();
                }
                else if (IsNumericOperand(_input[_idx]) || IsAlphaOperand(_input[_idx]))
                {
                    FillCurrentOperandToken();
                    OperandWork();
                }
                else if (IsOperator(_input[_idx]))
                {
                    OperatorWork();
                }
                else if (_input[_idx] == '(')
                {
                    ParseParenthesesContent();
                }
                else
                {
                    throw new Exception("Error: Unknown token");
                }
            }

            FinalizeExpressionStackFilling();
            var expr = FinalizeParsing();
            CheckParsingComplete();
            return expr;
        }


        // White space case
        private bool IsWhiteSpace(char c)
        {
            return (c == ' ' ||
                    c == '\r' ||
                    c == '\n' ||
                    c == '\t');
        }

        private void WhiteSpaceWork()
        {
            // Consume white spaces util next element of the input.
            while (_idx < _input.Length && IsWhiteSpace(_input[_idx]))
            {
                ++_idx;
            }
            _lastToken = Tokens.WhiteSpace;
        }


        // Operand case
        private bool IsNumericOperand(char c)
        {
            return ('0' <= c && c <= '9');
        }

        private bool IsAlphaOperand(char c)
        {
            return (('a' <= c && c <= 'z') ||
                    ('A' <= c && c <= 'Z'));
        }

        private void FillCurrentOperandToken()
        {
            // Check if the operand is well placed.
            if (_lastToken == Tokens.Operand)
            {
                throw new Exception("Syntax error: There are two different operands which follow");
            }

            // Reinitialize current operand token
            _currentOperandToken = "";

            // If the operand is numeric.
            if (IsNumericOperand(_input[_idx]))
            {
                while (_idx < _input.Length && IsNumericOperand(_input[_idx]))
                {
                    _currentOperandToken += _input[_idx];
                    ++_idx;
                }
            }

            // If the operand is alphabetical
            else
            {
                _currentOperandToken += _input[_idx];
                ++_idx;
            }
        }

        private void OperandWork()
        {
            // Create a new Operand object with the previously generated operand token
            // and push it into the operand stack.
            var operand = new Operand(_currentOperandToken);
            _operandStack.Push(operand);
            _lastToken = Tokens.Operand;
        }


        // Operator case
        private bool IsOperator(char c)
        {
            return (c == '+' ||
                    c == '*' ||
                    c == '/' ||
                    c == '%');
        }

        private int GetOperatorPrecedence(char op)
        {
            switch (op)
            {
                case '*':
                case '/':
                case '%':
                    return 2;
                case '+':
                    return 1;
                default:
                    return 0;
            }
        }

        private void OperatorWork()
        {
            // Check if the operator is well placed.
            if (_lastToken == Tokens.Operator || 
                _lastToken == Tokens.LeftParenthesis ||
                _lastToken == Tokens.Empty)
            {
                throw new Exception("Syntax error: Invalid token before an operator '" + _input[_idx] + "'");
            }

            // If there is an operator on the top of the stack and
            // the current operator have a lower or equal precedence than the operator on the top of the stack:
            //  - Create a new Expression Object.
            //  - Fill it with the avalable operands and operator (pop operand stack and stack).
            //  - Push it into the expression stack.
            if (_stack.Count > 0 &&
                IsOperator(_stack.Peek()) &&
                GetOperatorPrecedence(_input[_idx]) <= GetOperatorPrecedence(_stack.Peek()))
            {
                var expression = new Expression();
                if (_operandStack.Count >= 2)
                {
                    expression.Right = _operandStack.Pop();
                    expression.Left = _operandStack.Pop();
                    expression.Operator = _stack.Pop();
                    _expressionStack.Push(expression);
                }
                else
                {
                    throw new Exception("Error: Operand stack doesn't have enought elements.");
                }
            }

            // In any case, push the current operator in the stack.
            _stack.Push(_input[_idx]);
            
            // Update parser data
            ++_idx;
            _lastToken = Tokens.Operator;
        }


        // Parentheses case
        private void ParseParenthesesContent()
        {
            // Consume the opening parenthesis.
            ++_idx;

            // Find the expression between the parentheses.
            int parentheses = 0;
            string exprString = "";
            bool isParenthesesClosed = false;
            while (_idx < _input.Length)
            {
                if (_input[_idx] == '(')
                {
                    ++parentheses;
                }
                else if (_input[_idx] == ')' && parentheses > 0)
                {
                    --parentheses;
                }
                else if (_input[_idx] == ')' && parentheses == 0)
                {
                    isParenthesesClosed = true;
                    break;
                }
                exprString += _input[_idx];
                ++_idx;
            }
            if (!isParenthesesClosed)
            {
                throw new Exception("Syntax error: Ivalid parentheses.");
            }

            // If there is an expression between parentheses:
            //  - Generate recursively an Expression object and put it into
            //    the expression stack.
            if (exprString.Length > 0)
            {
                Prefixer prefixer = new Prefixer(exprString);
                _expressionStack.Push(prefixer.DoParse());
            }

            // Update parser data (Consume closing parenthesis)
            ++_idx;
            _lastToken = Tokens.LeftParenthesis;
        }


        // Finalization
        private void FinalizeExpressionStackFilling()
        {
            // Generate expressions with the remaining operators and operands in their stack
            // and push them into the expression stack.
            while (_operandStack.Count >= 2 && _stack.Count > 0)
            {
                var expr = new Expression();
                expr.Operator = _stack.Pop();
                expr.Right = _operandStack.Pop();
                expr.Left = _operandStack.Pop();
                _expressionStack.Push(expr);
            }
        }

        private IOperand FinalizeParsing()
        {
            // Check if the operand stack contains a valid number of elements.
            if (_operandStack.Count > 1)
            {
                throw new Exception("Error: Operand stack contain too many elements.");
            }

            // If the expression stack is empty and the operand stack contain 1 element:
            //  - Return the element in operand stack;
            if (_expressionStack.Count == 0 && _operandStack.Count == 1)
            {
                return _operandStack.Pop();
            }

            // Generate the final expression with the remaining stack elements
            else
            {
                // While there is element in the expression stack :
                IOperand expr = null;
                while (_expressionStack.Count > 0)
                {
                    // If expr have not been yet generated:
                    if (expr == null)
                    {
                        // If there is still operand into the operand stack:
                        //  - Create a new Expression object.
                        //  - Fill it with the first operator of the stack, the remaining operand
                        //    and the expression on the top of the expression stack.
                        if (_operandStack.Count == 1)
                        {
                            Expression tmp = new Expression();
                            tmp.Operator = _stack.Pop();
                            tmp.Left = _operandStack.Pop();
                            tmp.Right = _expressionStack.Pop();
                            expr = tmp;
                        }
                        // If the operand stack is empty:
                        //  - Fill
                        else if (_operandStack.Count == 0)
                        {
                            expr = _expressionStack.Pop();
                        }
                    }

                    // If expr have already been generated :
                    //  - Generate a new Expression object with the operator on the top of the stack,
                    //    the previously generated expression and the expression on the top of the expression stack.
                    else if (_stack.Count > 0)
                    {
                        Expression tmp = new Expression();
                        tmp.Operator = _stack.Pop();
                        tmp.Left = _expressionStack.Pop();
                        tmp.Right = expr;
                        expr = tmp;
                    }
                }

                return expr;
            }
        }

        private void CheckParsingComplete()
        {
            if (_stack.Count > 0 ||
                _operandStack.Count > 0 ||
                _expressionStack.Count > 0)
            {
                throw new Exception("Error: Parsing failed.");
            }
        }
    }
}
