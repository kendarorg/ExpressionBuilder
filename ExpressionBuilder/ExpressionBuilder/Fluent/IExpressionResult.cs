using System.Linq.Expressions;

namespace ExpressionBuilder.Fluent
{
	public interface IExpressionResult
	{
		LambdaExpression ToExpression();
		TData ToLambda<TData>() where TData : class;
	}
}
