/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a raw JSON string.
  /// </summary>
  public class JRaw : JValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="JRaw"/> class from another <see cref="JRaw"/> object.
    /// </summary>
    /// <param name="other">A <see cref="JRaw"/> object to copy from.</param>
    public JRaw(JRaw other)
      : base(other)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JRaw"/> class.
    /// </summary>
    /// <param name="rawJson">The raw json.</param>
    public JRaw(string rawJson)
      : base(rawJson, JTokenType.Raw)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="JRaw"/> with the content of the reader's current token.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of <see cref="JRaw"/> with the content of the reader's current token.</returns>
    public static JRaw Create(JsonReader reader)
    {
      using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
      using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
      {
        jsonWriter.WriteToken(reader);

        return new JRaw(sw.ToString());
      }
    }

    internal override JToken CloneToken()
    {
      return new JRaw(this);
    }

    //public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
    //{
    //  writer.WriteRawValue(Value);
    //}

    //internal override JToken CloneToken()
    //{
    //  throw new NotImplementedException();
    //}

    //internal override bool DeepEquals(JToken node)
    //{
    //  JRaw other = node as JRaw;
    //  if (other == null)
    //    return false;

    //  return (this == other || string.Equals(Value, other.Value, StringComparison.Ordinal));
    //}

    //public override JTokenType Type
    //{
    //  get { return JTokenType.Raw; }
    //}

    //public override bool HasValues
    //{
    //  get { return false; }
    //}

    //internal override int GetDeepHashCode()
    //{
    //  return Value.GetHashCode();
    //}
  }
}