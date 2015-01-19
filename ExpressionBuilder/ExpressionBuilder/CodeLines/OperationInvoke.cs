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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Operations;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.CodeLines
{
	public class OperationInvoke : IOperation, ICodeLine
	{
		internal IOperation Variable;
		internal string MethodName;
		internal IOperation[] Parameters;
		internal readonly Type StaticDataType;

		public OperationInvoke(IOperation variable, string methodName, IOperation[] parameters)
		{
			Variable = variable;
			MethodName = methodName;
			Parameters = parameters;
		}

		public OperationInvoke(Type dataType, string methodName, IOperation[] parameters)
		{
			MethodName = methodName;
			Parameters = parameters;
			StaticDataType = dataType;
		}

		public string ToString(ParseContext context)
		{
			var result = string.Empty;
			if (StaticDataType == null)
			{
				result = Variable.ToString(context);
			}
			else
			{
				result = ReflectionUtil.TypeToString(StaticDataType);
			}
			result += "." + MethodName + "(";
			for (int i = 0; i < Parameters.Length; i++)
			{
				if (i > 0) result += ", ";
				result += Parameters[i].ToString(context);
			}
			return result + ")";
		}



		private List<Type> _paramTypes;

		public void PreParseExpression(ParseContext context)
		{
			_paramTypes = new List<Type>();
			for (int i = 0; i < Parameters.Length; i++)
			{
				Parameters[i].PreParseExpression(context);
				_paramTypes.Add(Parameters[i].ParsedType);
			}

			ParsedType = null;
		}
		public Type ParsedType { get; private set; }

		public Expression ToExpression(ParseContext context)
		{
			var pars = new List<Expression>();

			foreach (var param in Parameters)
			{
				pars.Add(param.ToExpression(context));
			}

			Type type = StaticDataType;
			if (StaticDataType == null)
			{
				type = Variable.ParsedType;
				if (Variable is OperationVariable)
				{
					var variable = context.GetVariable(((OperationVariable)Variable).Name);
					type = variable.DataType;
				}
			}

			var method = ReflectionUtil.GetMethod(type, MethodName, _paramTypes);

			if (method.GoodFrom >= 0)
			{
				var startDefault = method.GoodFrom;
				while (startDefault < method.ParamValues.Count)
				{
					pars.Add(Operation.Constant(method.ParamValues[method.GoodFrom]).ToExpression(context));
					startDefault++;
				}
			}
			var my = (MethodInfo)method.Method;
			if ((my.Attributes & MethodAttributes.Static) == 0)
			{
				return Expression.Call(Variable.ToExpression(context), method.Method as MethodInfo, pars);
			}
			return Expression.Call(method.Method as MethodInfo, pars);
		}
	}
}