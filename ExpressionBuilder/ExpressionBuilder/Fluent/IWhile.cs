
namespace ExpressionBuilder.Fluent
{
	public interface IWhile
	{
		ICodeLine Do(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
	}
}
