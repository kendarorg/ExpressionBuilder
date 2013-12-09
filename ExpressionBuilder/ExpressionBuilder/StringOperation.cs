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
	}
}
