
namespace ExpressionBuilder.Fluent
{
	public interface IFunctionReturn : IExpressionResult
	{
		IExpressionResult Returns(string variableName);
	}
}
