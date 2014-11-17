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

namespace ExpressionBuilder.Parser
{
	public class ParseContext
	{
		public ParseContext()
		{
			_parseLevels = new List<ParseLevel>();
		}

		internal Expression Return;
		private readonly List<ParseLevel> _parseLevels;
		internal string ReturnVariable;

		internal ParseLevel AddLevel()
		{
			var pl = new ParseLevel(this);
			_parseLevels.Add(pl);
			return pl;
		}

		internal int Level
		{
			get
			{
				return Count - 1;
			}
		}

		internal int Count
		{
			get
			{
				return _parseLevels.Count;
			}
		}

		internal ParseLevel Current
		{
			get
			{
				return _parseLevels[Level];
			}
		}

		internal bool HasVariable(Variable var)
		{
			int i = Count - 1;
			while (i >= 0)
			{
				if (_parseLevels[i].HasVariable(var.Name))
				{
					return true;
				}
				i--;
			}
			return false;
		}

		internal string Pad
		{
			get
			{
				return GetPad(Level + 1);
			}
		}

		internal LabelTarget ReturnLabel;

		internal string GetPad(int level)
		{
			var res = "";
			while (level >= 0)
			{
				res += " ";
				level--;
			}
			return res;
		}

		internal Variable GetVariable(string name)
		{
			int i = Count - 1;
			while (i >= 0)
			{
				if (_parseLevels[i].HasVariable(name))
				{
					return _parseLevels[i].GetVariable(name);
				}
				i--;
			}
			throw new Exception("Variable not found "+name);
		}

		public void RemoveLevel()
		{
			_parseLevels.RemoveAt(Level);
		}
	}
}
