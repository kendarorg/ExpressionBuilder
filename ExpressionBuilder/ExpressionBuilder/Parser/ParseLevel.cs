using System;
using System.Collections.Generic;

namespace ExpressionBuilder.Parser
{
	internal class ParseLevel
	{
		private readonly ParseContext _parseContext;
		private readonly Dictionary<string, Variable> _variables;

		public ParseLevel(ParseContext parseContext)
		{
			_variables = new Dictionary<string, Variable>(StringComparer.InvariantCultureIgnoreCase);
			_parseContext = parseContext;
		}

		internal void AddVariable(Variable var)
		{
			if (_parseContext.HasVariable(var)) throw new Exception("Duplicate variable");
			_variables.Add(var.Name, var);
		}

		internal bool HasVariable(string name)
		{
			return _variables.ContainsKey(name);
		}

		internal Variable GetVariable(string name)
		{
			return _variables[name];
		}
	}
}