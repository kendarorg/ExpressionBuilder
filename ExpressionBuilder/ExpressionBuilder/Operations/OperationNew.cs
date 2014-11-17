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
