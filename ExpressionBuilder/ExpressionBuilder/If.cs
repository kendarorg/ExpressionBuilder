// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
