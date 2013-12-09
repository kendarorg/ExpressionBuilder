using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationNew : ILeftRightable
	{
		internal Type DataType;
		internal IRightable[] Variables;

		public OperationNew(Type dataType, IRightable[] variables)
		{
			DataType = dataType;
			Variables = variables;
		}

		public string ToString(ParseContext context)
		{
			var result = "new " + ReflectionUtil.TypeToString(DataType) + "(";
			for (int i = 0; i < Variables.Length; i++)
			{
				if (i > 0) result += ", ";
				result += Variables[i].ToString(context);
			}
			return result + ")";
		}

		private List<Type> _constructorTypes;

		public void PreParseExpression(ParseContext context)
		{
			_constructorTypes = new List<Type>();
			foreach (var param in Variables)
			{
				param.PreParseExpression(context);
				_constructorTypes.Add(param.ParsedType);
			}
			ParsedType = DataType;
		}

		public Type ParsedType { get; private set; }


		public Expression ToExpression(ParseContext context)
		{
			var pars = new List<Expression>();

			foreach (var param in Variables)
			{
				pars.Add(param.ToExpression(context));
			}
			var constructor = ReflectionUtil.GetConstructor(DataType, _constructorTypes);
			if (constructor.GoodFrom >= 0)
			{
				var startDefault = constructor.GoodFrom;
				while (startDefault < constructor.ParamValues.Count)
				{
					pars.Add(Operation.Constant(constructor.ParamValues[constructor.GoodFrom]).ToExpression(context));
					startDefault++;
				}
			}

			return Expression.New(constructor.Method as ConstructorInfo, pars);
		}
	}
}
