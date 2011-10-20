/*

SMDBehavior project.
Copyright (C) 2009  Fabrice Michellonet (Fabrice dot Michellonet at gmail dot com)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/
using System;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace FM.WCF.SMDBehavior
{
    /// <summary>
    /// SMDBehaviorExtensionElement is in charge of holding the setting of this service behavior.
    /// It is also responsible of the instantiation of the <seealso cref="SMDBehavior"/>.
    /// SMDBehaviorExtensionElement extends <seealso cref="BehaviorExtensionElement"/>.
    /// </summary>
    public class SMDBehaviorExtensionElement : BehaviorExtensionElement
    {

        /// <summary>
        /// Get or set a boolean indicating wheter the json response should be indented.
        /// The default value is false.
        /// </summary>
        [ConfigurationProperty("Indent", DefaultValue = false, IsRequired = false)]
        public bool Indent
        {
            get { return (bool)base["Indent"]; }
            set { base["Indent"] = value; }
        }

        /// <summary>
        /// Get or set the string representing the url template that should match
        /// during json schema (smd) request.
        /// The default value is "/?smd"
        /// </summary>
        [ConfigurationProperty("UrlTemplate", DefaultValue = "/?smd", IsRequired = false)]
        public string UrlTemplate {
            get { return (string)base["UrlTemplate"]; }
            set { base["UrlTemplate"] = value; }
        }
        
        /// <summary>
        /// Return the type of the behavior configured in the xml config section.
        /// </summary>
        public override Type BehaviorType
        {
            get { return typeof(SMDBehavior); }
        }

        /// <summary>
        /// Instantiate the SMDBehavior and initialize with its configuration.
        /// </summary>
        /// <returns>An SMDBehavior.</returns>
        protected override object CreateBehavior()
        {
            return new SMDBehavior() { 
                IndentResponse = this.Indent,
                UrlTemplate = this.UrlTemplate
            };
        }
    }
}
