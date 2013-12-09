using System;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.Operations
{
	public class OperationConst : IRightable
	{
		private readonly object _value;

		public OperationConst(object value)
		{
			_value = value;
		}

		public string ToString(ParseContext context)
		{
			if (_value == null) return "null";
			var type = _value.GetType();
			if (type.IsValueType || type.IsEnum)
			{
				return _value.ToString();
			}
			return "\"" + _value + "\"";
		}

		public Expression ToExpression(ParseContext context)
		{
			return Expression.Constant(_value);
		}

		public void PreParseExpression(ParseContext context)
		{
			if (_value == null)
				ParsedType = typeof(object);
			else
				ParsedType = _value.GetType();
		}

		public Type ParsedType { get; private set; }
	}
}