using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationInvokeReturn : IRightable
	{
		internal IOperation Variable;
		internal readonly Type StaticDataType;
		internal string MethodName;
		internal IOperation[] Parameters;

		public OperationInvokeReturn(IOperation variable, string methodName, IOperation[] parameters)
		{
			Variable = variable;
			MethodName = methodName;
			Parameters = parameters;
		}

		public OperationInvokeReturn(Type dataType, string methodName, IOperation[] parameters)
		{
			StaticDataType = dataType;
			MethodName = methodName;
			Parameters = parameters;
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

			var methodInfo = type.GetMethod(MethodName, _paramTypes.ToArray());
			ParsedType = methodInfo.ReturnType;
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
					var variable = context.GetVariable(((OperationVariable) Variable).Name);
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

			if (StaticDataType == null)
			{
				return Expression.Call(Variable.ToExpression(context), method.Method as MethodInfo, pars);
			}
			return Expression.Call(method.Method as MethodInfo, pars);
		}
	}
}