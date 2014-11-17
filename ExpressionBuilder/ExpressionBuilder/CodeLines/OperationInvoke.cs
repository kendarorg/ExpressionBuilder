// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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