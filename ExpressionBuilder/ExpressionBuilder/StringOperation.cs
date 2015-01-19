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
