using System;

namespace ExpressionBuilder.Fluent
{
	public interface IFunctionParameter
	{
		IBodyOrParameter WithParameter(Type type, string name);
		IBodyOrParameter WithParameter<TData>(string name);
	}
}
