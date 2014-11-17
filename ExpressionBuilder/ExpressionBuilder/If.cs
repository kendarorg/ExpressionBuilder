// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.CodeLines;

namespace ExpressionBuilder
{
	public class If : IIf, IIfThen
	{
		internal If ParentIf;
		internal List<ICodeLine> ThenCodeLines;
		internal List<ICodeLine> ElseCodeLines;
		internal Condition Condition;

		internal If(Condition condition, If parentIf = null)
		{
			ParentIf = parentIf;
			if (condition == null) throw new ArgumentException();
			Condition = condition;
			ThenCodeLines = new List<ICodeLine>();
			ElseCodeLines = new List<ICodeLine>();
		}

		public IIfThen Then(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
		{
			ThenCodeLines.Add(firstCodeLine);
			foreach (var codeLine in codeLines)
			{
				ThenCodeLines.Add(codeLine);
			}
			return this;
		}

		public IIf ElseIf(Condition elseIfCondition)
		{
			var elseIf = new If(elseIfCondition, this);
			ElseCodeLines.Add(elseIf);
			return elseIf;
		}

		public ICodeLine Else(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
		{
			ElseCodeLines.Add(firstCodeLine);
			foreach (var codeLine in codeLines)
			{
				ElseCodeLines.Add(codeLine);
			}
			return this;
		}

		public string ToString(ParseContext context)
		{
			var result = "if(" + Condition.ToString(context) + ")\n";
			result += context.Pad + "{\n";
			context.AddLevel();

			foreach (var line in ThenCodeLines)
			{
				var createVariable = line as CreateVariable;
				if (createVariable != null)
				{
					createVariable.DefaultInitialize(context);
				}
				result += context.Pad + line.ToString(context) + ";\n";
			}

			context.RemoveLevel();
			result += context.Pad + "}";
			if (ElseCodeLines.Count > 0)
			{
				result += context.Pad + "\n";
				result += context.Pad + "else\n";
				result += context.Pad + "{\n";
				context.AddLevel();

				foreach (var line in ElseCodeLines)
				{
					var createVariable = line as CreateVariable;
					if (createVariable != null)
					{
						createVariable.DefaultInitialize(context);
					}
					result += context.Pad + line.ToString(context) + ";\n";
				}

				context.RemoveLevel();
				result += context.Pad + "}";
			}
			return result;
		}

		public void PreParseExpression(ParseContext context)
		{
			//var pl = context.Current;
			Condition.PreParseExpression(context);
			context.AddLevel();

			foreach (var line in ThenCodeLines)
			{
				line.PreParseExpression(context);
			}

			context.RemoveLevel();

			if (ElseCodeLines.Count > 0)
			{
				context.AddLevel();

				foreach (var line in ElseCodeLines)
				{
					line.PreParseExpression(context);
				}

				context.RemoveLevel();
			}
		}

		public Type ParsedType { get; private set; }

		public Expression ToExpression(ParseContext context)
		{
			var conditionExpression = Condition.ToExpression(context);
			context.AddLevel();

			var thenLine = new List<Expression>();
			var listOfThenVars = new List<ParameterExpression>();
			foreach (var line in ThenCodeLines)
			{
				var expLine = line.ToExpression(context);

				var createVariable = line as CreateVariable;
				if (createVariable != null)
				{
					listOfThenVars.Add((ParameterExpression)expLine);
					expLine = createVariable.DefaultInitialize(context);
				}
				thenLine.Add(expLine);
			}
			var thenBlock = Expression.Block(listOfThenVars.ToArray(), thenLine);

			context.RemoveLevel();

			var elseLine = new List<Expression>();
			if (ElseCodeLines.Count > 0)
			{
				context.AddLevel();
				var listOfElseVars = new List<ParameterExpression>();
				foreach (var line in ElseCodeLines)
				{
					var expLine = line.ToExpression(context);

					var createVariable = line as CreateVariable;
					if (createVariable != null)
					{
						listOfElseVars.Add((ParameterExpression)expLine);
						expLine = createVariable.DefaultInitialize(context);
					}
					elseLine.Add(expLine);
				}

				context.RemoveLevel();
				var elseBlock = Expression.Block(listOfElseVars, elseLine);
				return Expression.IfThenElse(conditionExpression, thenBlock, elseBlock);
			}
			return Expression.IfThen(conditionExpression, thenBlock);
		}
	}
}
