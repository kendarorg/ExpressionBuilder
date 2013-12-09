
namespace ExpressionBuilder.Fluent
{
	public interface IIf
	{
		IIfThen Then(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
	}
}
