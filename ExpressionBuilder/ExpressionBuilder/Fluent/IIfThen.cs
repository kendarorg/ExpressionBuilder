
namespace ExpressionBuilder.Fluent
{
	public interface IIfThen : ICodeLine
	{
		IIf ElseIf(Condition elseIfCondition);
		ICodeLine Else(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
	}
}
