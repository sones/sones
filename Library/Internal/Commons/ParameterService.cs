using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons
{
	/// <summary>
	/// Provides static method for verifying parameter values.
	/// </summary>
	public static class ParameterService
	{
		/// <summary>
		/// Ensures the specified parameter value is not NULL and throws an ArgumentNullException in case it is NULL.
		/// </summary>
		/// 
		/// <typeparam name="TEnum">The enum type.</typeparam>
		/// 
		/// <param name="paramValue">The value to be verified.</param>
		/// <param name="paramName">The name of the parameter to be verified.</param>
		/// 
		/// <exception cref="System.ArgumentException">
		///		The specified paramValue is not a defined member of the TEnum enumeration.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		///		The specified TEnum is not a is not an enum type.
		/// </exception>
		public static void EnsureEnumMember<TEnum>(TEnum paramValue, String paramName) where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			if(!typeof(TEnum).IsEnum)
				throw new InvalidOperationException("Type must be an enum!");

			if(typeof(TEnum).IsEnumDefined(paramValue))
				return;

			throw new ArgumentException(string.Format("The specified value '{0}' is not a defined member of the {1} enumeration.", paramValue, typeof(TEnum).Name), paramName);
		}

		/// <summary>
		/// Ensures the specified parameter value is not NULL and throws an ArgumentNullException in case it is NULL.
		/// </summary>
		/// 
		/// <param name="paramValue">The value to be verified.</param>
		/// <param name="paramName">The name of the parameter to be verified.</param>
		/// 
		/// <exception cref="System.ArgumentNullException">
		///		The specified paramValue is NULL.
		/// </exception>
		public static void EnsureNotNull(object paramValue, String paramName)
		{
			if(paramValue != null)
				return;

			throw new ArgumentNullException(paramName);
		}
	}
}
