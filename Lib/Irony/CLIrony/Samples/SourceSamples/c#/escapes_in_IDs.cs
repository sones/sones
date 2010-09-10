/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

//example from c# spec, page 93
namespace Sample {
	class @class 
	{ 
	  public static void @static(bool @bool) { 
	  if (@bool) 
	   System.Console.WriteLine("true"); 
	  else 
	   System.Console.WriteLine("false"); 
	 }  
	} 
	class Class1 
	{ 
	  static void M() { 
	  cl\u0061ss.st\u0061tic(true); 
	 } 
	}

}

/*  code comment from spec:
[the code]  defines a class named “class” with a static method named “static” that takes a parameter named 
“bool”. Note that since Unicode escapes are not permitted in keywords, the token “cl\u0061ss” is an 
identifier, and is the same identifier as “@class”. end example]
*/