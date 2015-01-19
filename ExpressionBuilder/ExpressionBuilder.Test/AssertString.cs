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
using System.ComponentModel;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	public static class AssertString
	{
		public static void AreEqual(string expected, string actual)
		{
			expected = expected.Replace("\r\n", "\n");
			actual = actual.Replace("\r\n", "\n");
			if (expected.Length == actual.Length)
			{
				for (var i = 0; i < expected.Length; i++)
				{
					var expectedChar = (int) expected[i];
					var actualChar = (int)actual[i];
					if (actualChar != expectedChar)
					{
						break;
					}
				}
				return;
			}
			throw new AssertFailedException(string.Format("AssertString.AreEqual\nExpected <{0}>\nActual  <{1}>", expected,
				actual));
		}

		public static string UTF8ToAscii(string text)
		{
			var utf8 = Encoding.UTF8;
			Byte[] encodedBytes = utf8.GetBytes(text);
			Byte[] convertedBytes = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, encodedBytes);

			return Encoding.ASCII.GetString(convertedBytes);
		}
	}
}
