using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using WCFExtras.Wsdl.Documentation;

namespace WCFExtras.Utils
{
    /// <summary>
    /// allows customizing a comment
    /// </summary>
    /// <param name="comment">The comment to format</param>
    /// <returns>True if the formatting was handled</returns>
    public delegate bool FormatComment(ref string comment);

    public static class XmlCommentsUtils
    {
        static Dictionary<string, XmlDocument> xmlDocCache = new Dictionary<string, XmlDocument>();
        static Dictionary<MemberInfo, string> memberCommentCache = new Dictionary<MemberInfo, string>();

        private static XmlDocument TryLoadFromLocation(string fileName)
        {
            XmlDocument doc = null;
            if (xmlDocCache.TryGetValue(fileName, out doc))
                return doc;
            FileInfo fi = new FileInfo(Path.ChangeExtension(fileName, "xml"));
            if (fi.Exists)
            {
                doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(fi.FullName);
            }
            xmlDocCache[fileName] = doc;
            return doc;
        }

        private static string GetFormattedComment(MemberInfo member, XmlNode commentNode, XmlCommentFormat format)
        {
            string result;
            if (format == XmlCommentFormat.Default)
            {
                result = commentNode.InnerXml.Trim('\n', '\r', ' ');
            }
            else
            {
                string summary = GetTextFromNode(commentNode, "summary");
                List<string> paramList = GetParams(commentNode);
                string returnValue = GetTextFromNode(commentNode, "returns"); ;

                List<string> comments = new List<string>();
                if (!string.IsNullOrEmpty(summary))
                    comments.Add(summary);
                comments.AddRange(paramList);
                if (!string.IsNullOrEmpty(returnValue))
                    comments.Add("@return " + returnValue);

                result = String.Join("\n", comments.ToArray());
            }

            bool handled = false;
            if (FormatComment != null)
                handled = FormatComment(ref result); //Allows custimizing the formatted comment
            if (!handled)
                result = FixReferences(member, result);
            return result;
        }

        private static string FixReferences(MemberInfo member, string result)
        {
            string fullMemberName = GetFullMemberName(member);
            result = Regex.Replace(result, "\"[P|T|M|F]:([^>]*)\"", delegate(Match match)
            {
                string matchValue = match.Value;
                string textToReplace = match.Groups[1].Value;
                string replacement = RemoveCommonParts(fullMemberName, textToReplace);
                return matchValue.Replace(textToReplace, replacement);
            });
            return result;
        }

        private static string RemoveCommonParts(string memberName, string reference)
        {
            reference = Regex.Replace(reference, "\\(.*\\)", "()");

            int maxLen = Math.Min(reference.LastIndexOf('.'), memberName.Length);
            int index = 0;
            while (index <= maxLen && memberName[index] == reference[index])
                index++;

            return reference.Substring(index);
        }

        private static string GetFullMemberName(MemberInfo member)
        {

            if (member is Type)
                return GetXmlCommentName((Type)member);
            else
                return GetXmlCommentName(member.DeclaringType) + "." + member.Name;

        }

        private static List<string> GetParams(XmlNode commentNode)
        {
            List<string> result = new List<string>();
            foreach (XmlNode paramNode in commentNode.SelectNodes("param"))
            {
                string paramText = paramNode.InnerXml;
                if (!string.IsNullOrEmpty(paramText))
                {
                    string paramName = string.Empty;
                    if (paramNode.Attributes["name"] != null)
                        paramName = paramNode.Attributes["name"].Value;
                    result.Add("@param " + paramName + " " + SingleLine(paramText));
                }
            }
            return result;
        }

        private static string GetTextFromNode(XmlNode commentNode, string query)
        {
            XmlNode node = commentNode.SelectSingleNode(query);
            if (node != null)
                return SingleLine(node.InnerXml);
            return null;
        }

        private static string SingleLine(string s)
        {
            string[] lines = s.Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(" ", lines);
        }

        private static XmlNode GetCommentNodeForMember(XmlDocument commentsDoc, MemberInfo member)
        {
            string xmlCommentMember = GetXmlCommentMemberName(member);
            string xpathQuery = String.Format("doc/members/member[@name=\"{0}\"]", xmlCommentMember);
            return commentsDoc.SelectSingleNode(xpathQuery);
        }

        private static string GetXmlCommentMemberName(MemberInfo mi)
        {
            string memberType = string.Empty;
            string subType = mi.Name;
            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    memberType = "P";
                    break;

                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    memberType = "T";
                    subType = null;
                    break;

                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    memberType = "M";
                    break;

                case MemberTypes.Event:
                    memberType = "E";
                    break;

                case MemberTypes.Field:
                    memberType = "F";
                    break;
            }
            string declaringType;
            string[] paramStrs = null;
            if (mi is MethodInfo)
            {
                ParameterInfo[] parameters = ((MethodInfo)mi).GetParameters();
                paramStrs = new string[parameters.Length];
                int i = 0;
                foreach (ParameterInfo pi in parameters)
                {
                    paramStrs[i++] = GetXmlCommentName(pi.ParameterType);
                }
                declaringType = GetXmlCommentName(mi.DeclaringType);
            }
            else if (mi is Type)
            {
                declaringType = GetXmlCommentName((Type)mi);
            }
            else
                declaringType = GetXmlCommentName(mi.ReflectedType);
            return FormatMemberName(memberType, declaringType, subType, paramStrs);
        }

        private static string GetXmlCommentName(Type type)
        {
            string result;
            if (type.IsGenericType)
            {
                result = type.GetGenericTypeDefinition().FullName;
                result = result.Substring(0, result.LastIndexOf('`'));
                if (!type.IsGenericTypeDefinition)
                {
                    string genericParams = String.Join(",", type.GetGenericArguments().Select(t => GetXmlCommentName(t)).ToArray());
                    result = result + "{" + genericParams + "}";
                }
            }
            else
                result = type.ToString();
            return result.Replace('&', '@').Replace('+', '.');
        }

        static string FormatMemberName(string memberType, string mainType, string subType, string[] prms)
        {
            if (string.IsNullOrEmpty(subType))
                return string.Format("{0}:{1}", memberType, mainType);
            else if (prms == null || prms.Length == 0)
                return string.Format("{0}:{1}.{2}", memberType, mainType, subType);
            else
                return string.Format("{0}:{1}.{2}({3})", memberType, mainType, subType, String.Join(",", prms));

        }

        private static bool ContainsDotNetXMLCommentTags(string documentation)
        {
            return
                documentation.Contains("<summary>") ||
                documentation.Contains("<param") ||
                documentation.Contains("<returns>") ||
                documentation.Contains("<remarks>");
        }

        private static IEnumerable<string> ExtractLines(string documentation, bool wrapLongLines)
        {
            List<string> lines = new List<string>();
            string line;
            using (TextReader reader = new StringReader(documentation))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (wrapLongLines)
                    {
                        foreach (string wrappedLine in WordWrapLine(line, 105))
                        {
                            lines.Add(wrappedLine);
                        }
                    }
                    else
                        lines.Add(line);
                }
            }
            return lines;
        }

        private static IEnumerable<string> WordWrapLine(string line, int maxLen)
        {
            List<string> lines = new List<string>();
            StringBuilder sb = new StringBuilder();

            string[] words = line.Split(' ');
            foreach (string word in words)
            {
                if ((sb.Length > 0) && (sb.Length + word.Length) >= maxLen)
                {
                    lines.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                sb.Append(word);
                sb.Append(' ');
            }
            lines.Add(sb.ToString());

            return lines;
        }

        public static void ClearCache()
        {
            xmlDocCache.Clear();
            memberCommentCache.Clear();
        }

        public static XmlDocument LoadXmlComments(Type type)
        {
            return LoadXmlComments(type, false);
        }

        public static XmlDocument LoadXmlComments(Type type, bool throwIfNotFound)
        {
            Assembly assembly = type.Assembly;
            XmlDocument doc = null;
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                string fileName = assembly.Location;
                doc = TryLoadFromLocation(fileName);
                if (doc == null)
                {
                    fileName = new Uri(assembly.CodeBase).LocalPath;
                    doc = TryLoadFromLocation(fileName);
                }
            }
            if (doc == null && throwIfNotFound)
            {
                throw new ApplicationException("XML documentation file for " + Path.GetFileName(assembly.Location) + " was not found. Make sure the XML documentation option is enabled in the project properties for that assembly.");
            }
            return doc;
        }

        public static string GetFormattedComment(XmlDocument commentsDoc, MemberInfo member, XmlCommentFormat format)
        {
            string result;
            if (memberCommentCache.TryGetValue(member, out result))
                return result;

            XmlNode commentNode = GetCommentNodeForMember(commentsDoc, member);
            if (commentNode != null)
                result = GetFormattedComment(member, commentNode, format);
            else
                result = null;

            memberCommentCache[member] = result;
            return result;
        }

        public static IEnumerable<string> ParseAndReformatComment(string documentation, XmlCommentFormat format, bool wrapLongLines)
        {
            if (format == XmlCommentFormat.Default || ContainsDotNetXMLCommentTags(documentation))
                return ExtractLines(documentation, wrapLongLines);
            else
            {
                string summary = string.Empty;
                Match m = Regex.Match(documentation, "^@", RegexOptions.Multiline);
                if (m.Success)
                {
                    summary = documentation.Substring(0, m.Index);
                    if (summary.Length > 0)
                        summary = "<summary>\r\n" + summary + "\r\n</summary>\r\n";
                    documentation = documentation.Substring(m.Index);

                    //change "@return {some text}" to "<returns>{some text}</returns>"
                    documentation = Regex.Replace(documentation, "^@return\\s+(?<text>.*)$", "<returns>${text}</returns>", RegexOptions.Multiline);
                    //change "@param paramname {some text}" to "<param name="paramname">{some text}</param>"
                    documentation = Regex.Replace(documentation, "^@param\\s+(?<name>\\S+)\\s+(?<text>.*)$", "<param name=\"${name}\">${text}</param>", RegexOptions.Multiline);
                }
                else
                    documentation = "<summary>\r\n" + documentation + "\r\n</summary>\r\n"; ;

                return ExtractLines(summary + documentation, true);
            }

        }

        public static event FormatComment FormatComment;
    }
}
