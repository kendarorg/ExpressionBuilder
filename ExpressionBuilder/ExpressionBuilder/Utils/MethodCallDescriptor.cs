using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressionBuilder.Utils
{
	public class MethodCallDescriptor
	{
		public MethodCallDescriptor()
		{
			ParamTypes = new List<Type>();
			ParamValues = new List<object>();
			GoodFrom = 0;
		}

		public MethodBase Method { get; set; }
		public List<Type> ParamTypes { get; set; }
		public List<object> ParamValues { get; set; }
		public int GoodFrom { get; set; }
	}
}

