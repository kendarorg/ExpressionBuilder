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
