using System;
using System.Linq.Expressions;

namespace ExpressionBuilder
{
	public class Variable
	{
		internal Type DataType;
		internal string Name;
		internal bool Assignable;
		internal Expression Expression;

		public Variable(Type dataType, string name, bool assignable = true)
		{
			DataType = dataType;
			Name = name;
			Assignable = assignable;
		}
	}
}
