/* Code.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System;
using System.Reflection.Emit;

namespace Aki.SinglePlayer.Utils.Reflection.CodeWrapper
{
    public class Code
    {
        public Code(OpCode opCode)
        {
            OpCode = opCode;
            HasOperand = false;
        }

        public Code(OpCode opCode, object operandTarget)
        {
            OpCode = opCode;
            OperandTarget = operandTarget;
        }

        public Code(OpCode opCode, Type callerType)
        {
            OpCode = opCode;
            CallerType = callerType;
        }

        public Code(OpCode opCode, Type callerType, object operandTarget, Type[] parameters = null)
        {
            OpCode = opCode;
            CallerType = callerType;
            OperandTarget = operandTarget;
            Parameters = parameters;
        }

        public virtual Label? GetLabel()
        {
            return null;
        }

        public OpCode OpCode { get; }
        public Type CallerType { get; }
        public object OperandTarget { get; }
        public Type[] Parameters { get; }

        public bool HasOperand { get; } = true;
    }
}
