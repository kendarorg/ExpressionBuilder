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
using ExpressionBuilder.Fluent;

namespace ExpressionBuilder
{
	public static class CollectionOperation
	{
		public static IRightable CreateDictionary(Type key, Type value)
		{
			var dType = typeof(Dictionary<int, int>).MakeGenericType(new[] { key, value });
			return Operation.CreateInstance(dType);
		}

		public static IRightable CreateDictionary<TKey, TVal>()
		{
			return CreateDictionary(typeof(TKey), typeof(TVal));
		}

		public static IRightable CreateList(Type value)
		{
			var dType = typeof(List<int>).MakeGenericType(new[] { value });
			return Operation.CreateInstance(dType);
		}

		public static IRightable CreateList< TVal>()
		{
			return CreateList(typeof(TVal));
		}

		public static IRightable Count(IOperation variable)
		{
			return Operation.Get(variable, "Count");
		}

		public static IRightable Count(string variable)
		{
			return Count(Operation.Variable(variable));
		}

		public static IRightable Count(object value)
		{
			return Count(Operation.Constant(value));
		}
	}
}
