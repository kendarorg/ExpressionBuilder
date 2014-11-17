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