
namespace ExpressionBuilder.Fluent
{
	public interface IFunctionBody
	{
		IFunctionReturn WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
		//IFluentCodeLine WithFluentBody();
	}
}
