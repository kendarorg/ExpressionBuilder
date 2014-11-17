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
using System.Linq;
using ExpressionBuilder.Fluent;

namespace ExpressionBuilder
{
	public class StringOperation
	{

		public static IRightable ToString(IOperation variable)
		{
			return Operation.InvokeReturn(variable, "ToString");
		}

		public static IRightable ToString(string variable)
		{
			return ToString(Operation.Variable(variable));
		}

		public static IRightable ToStringConst(object value)
		{
			return ToString(Operation.Constant(value));
		}

		public static IRightable Compare(string lVariable, string rVariable, StringComparison comp = StringComparison.CurrentCulture)
		{
			var lValue = Operation.Variable(lVariable);
			var rValue = Operation.Variable(rVariable);
			return Compare(lValue, rValue, comp);
		}

		public static IRightable Compare(IOperation lValue, string rVariable, StringComparison comp = StringComparison.CurrentCulture)
		{
			var rValue = Operation.Variable(rVariable);
			return Compare(lValue, rValue, comp);
		}

		public static IRightable Compare(string lVariable, IOperation rValue, StringComparison comp = StringComparison.CurrentCulture)
		{
			var lValue = Operation.Variable(lVariable);
			return Compare(lValue, rValue, comp);
		}

		private static IRightable Compare(IOperation lValue, IOperation rValue, StringComparison comp)
		{
			return Operation.InvokeReturn<string>("Compare", lValue, rValue, Operation.Constant(comp));
		}

		public static IRightable Format(string formatString, params IOperation[] pars)
		{
			var formatPars = new List<IOperation>();
			formatPars.Add(Operation.Constant(formatString));
			formatPars.AddRange(pars);

			return Operation.InvokeReturn<string>("Format", formatPars.ToArray());
		}

		public static IRightable Length(IOperation variable)
		{
			return Operation.InvokeReturn(variable, "Length");
		}

		public static IRightable Length(string variable)
		{
			return ToString(Operation.Variable(variable));
		}

		public static IRightable Length(object value)
		{
			return ToString(Operation.Constant(value));
		}
	}
}
