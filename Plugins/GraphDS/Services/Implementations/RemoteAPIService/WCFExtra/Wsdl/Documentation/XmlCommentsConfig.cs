using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using WCFExtras.Utils;
using System.IO;

namespace WCFExtras.Wsdl.Documentation
{
    class XmlCommentsConfig : ConfigurationSection
    {
        [ConfigurationProperty("format", DefaultValue = XmlCommentFormat.Default)]
        public XmlCommentFormat Format
        {
            get
            {
                return (XmlCommentFormat)base["format"];
            }
            set
            {
                base["format"] = value;
            }
        }

        [ConfigurationProperty("wrapLongLines", DefaultValue = false)]
        public bool WrapLongLines
        {
            get
            {
                return (bool)base["wrapLongLines"];
            }
            set
            {
                base["wrapLongLines"] = value;
            }
        }

        [ConfigurationProperty("documentable", DefaultValue = false)]
        public bool Documentable
        {
            get
            {
                return (bool)base["documentable"];
            }
            set
            {
                base["documentable"] = value;
            }
        }


        public static XmlCommentsConfig GetConfiguration()
        {
            return GetConfiguration(null);
        }

        public static XmlCommentsConfig GetConfiguration(Configuration configuration)
        {
            XmlCommentsConfig config = null;
            if (configuration != null)
                config = configuration.GetSection("xmlComments") as XmlCommentsConfig;
            if (config == null)
            {
                config = ConfigurationManager.GetSection("xmlComments") as XmlCommentsConfig;
                if (config == null)
                {
                    string configFile = GetConfigFileFromCommandLine();
                    if (configFile != null && File.Exists(configFile))
                    {
                        Configuration c = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);
                        if (c != null)
                            config = c.GetSection("xmlComments") as XmlCommentsConfig;
                    }
                };
            }
            return config;
        }

        private static string GetConfigFileFromCommandLine()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                string[] args = arg.Split(new char[] { ':', '=' }, 2);
                if (args.Length == 2 && args[0].IndexOf("svcutilConfig", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return args[1];
                }
            }
            return null;
        }
    }

    class ImportOptions
    {
        public XmlCommentFormat Format { get; set; }
        public bool WrapLongLines { get; set; }
        public bool Documentable { get; set; }

        internal bool Initialized = false;
    }
}
