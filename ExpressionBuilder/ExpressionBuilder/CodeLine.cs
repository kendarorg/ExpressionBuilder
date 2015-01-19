// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Enums;
using ExpressionBuilder.Fluent;

namespace ExpressionBuilder
{
	public static class CodeLine
	{
		public static ICodeLine CreateVariable(Type dataType, string variableName)
		{
			return new CreateVariable(new Variable(dataType, variableName));
		}


		public static ICodeLine CreateVariable<TData>(string variableName)
		{
			return CreateVariable(typeof(TData), variableName);
		}

		public static ICodeLine AssignConstant(string lVariableName, object rConst, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), Operation.Constant(rConst), assignType);
		}

		public static ICodeLine AssignConstant(ILeftable lValue, object rConst, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(lValue, Operation.Constant(rConst), assignType);
		}

		public static ICodeLine Assign(ILeftable lValue, IRightable rValue, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return new Assign(lValue, rValue, assignType);
		}

		public static ICodeLine Assign(string lVariableName, string rVariableName, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), Operation.Variable(rVariableName), assignType);
		}

		public static ICodeLine Assign(ILeftable lValue, string rVariableName, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(lValue, Operation.Variable(rVariableName), assignType);
		}

		public static ICodeLine Assign(string lVariableName, IRightable rValue, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), rValue, assignType);
		}

		public static IIf CreateIf(Condition condition)
		{
			return new If(condition);
		}


		public static IWhile CreateWhile(Condition condition)
		{
			return new While(condition);
		}

		public static ICodeLine Return()
		{
			return new CreateReturn();
		}

		public static ICodeLine Nop
		{
			get
			{
				return new Nop();		
			}
		
		}
	}
}
