/*
 * IError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;

#endregion

namespace sones.Lib.ErrorHandling
{

    public interface IError
    {
        String      Message         { get; }
        StackTrace  StackTrace      { get; }
    }

}
