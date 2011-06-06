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
using System.Linq;
using System.Reflection;
using sones.Library.LanguageExtensions;

namespace sones.Library.Network.HttpServer
{
    
    /// <summary>
    /// A URL node which stores some childnodes and a callback
    /// </summary>
    public class UrlNode
    {

        public Dictionary<String, UrlNode> ChildNodes { get; set; }
        public MethodInfo Callback { get; set; }
        public Boolean NeedsExplicitAuthentication { get; set; }

        public UrlNode()
        {
            ChildNodes = new Dictionary<String, UrlNode>();
        }

    }

    /// <summary>
    /// The URLParser class which parses some URLs
    /// </summary>
    public class URLParser
    {

        #region data

        //NLOG: temporarily commented
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        private char[] _Separators;
        /// <summary>
        /// &lt;WebMethod, &lt;location, UrlNode&gt;&gt;
        /// </summary>
        private Dictionary<String, Dictionary<String, UrlNode>> _RootNodes;

        #endregion

        #region constructors

        public URLParser(char[] separators)
        {
            _RootNodes = new Dictionary<String, Dictionary<String, UrlNode>>();
            _Separators = separators;
        }

        #endregion

        #region Initialize the parser by adding some URL definitions

        /// <summary>
        /// Adds a url
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="method">The method which should be invoked for any match</param>
        /// <param name="needsExplicitAuthentication"></param>
        /// <param name="webMethod"></param>
        public void AddUrl(string url, MethodInfo method, bool needsExplicitAuthentication, string webMethod = "GET")
        {

            url = url.ToLower();
            String[] ValueArray = url.Split(_Separators);

            if (ValueArray.Length == 0)
            {
                return;
            }

            var node = new Dictionary<String, UrlNode>();
            if (_RootNodes.ContainsKey(webMethod))
            {
                node = _RootNodes[webMethod];
            }
            else
            {
                _RootNodes.Add(webMethod, node);
            }

            AddNode(node, ValueArray, method, url, 0, needsExplicitAuthentication);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlNodes"></param>
        /// <param name="urlParts"></param>
        /// <param name="methodInfo"></param>
        /// <param name="url">The uriginal url - this is needed to get the real parameter before splitting. Due to the split we lost the seperator char</param>
        /// <param name="posInUrl">The current pos in the url - this is needed to get the real parameter before splitting. Due to the split we lost the seperator char</param>
        private void AddNode(Dictionary<String, UrlNode> urlNodes, IEnumerable<String> urlParts, MethodInfo methodInfo, String url, Int32 posInUrl, Boolean needsExplicitAuthentication)
        {

            var val = urlParts.FirstOrDefault();

            if (val == null)
            {
                throw new ArgumentNullException("urlParts.First");
            }

            #region SpeedUp by removing all between {...}

            if (val.StartsWith("{") && val.EndsWith("}")) // something like /{....} or .{....} or ?{.....} will be changed to /{} .{} ?{} 
            {
                val = "{}";
            }

            #endregion

            UrlNode curUrlNode = null;

            #region Use an existing node or add a new one

            if (urlNodes.ContainsKey(val))
            {
                curUrlNode = urlNodes[val];
            }
            else
            {
                curUrlNode = new UrlNode();
                curUrlNode.NeedsExplicitAuthentication = needsExplicitAuthentication;
                urlNodes.Add(val, curUrlNode);
            }

            #endregion

            #region If there are some more parts proceed or set the methodInfo for the last part

            if (urlParts.Count() > 1)
            {
                // there are still some more parts of the URL
                AddNode(curUrlNode.ChildNodes, urlParts.Skip(1), methodInfo, url, posInUrl, needsExplicitAuthentication);
            }
            else
            {
                // this was the last one - take the methodInfo
                curUrlNode.Callback = methodInfo;
            }

            #endregion

        }

        #endregion

        #region Get a callback for an url on the parser

        /// <summary>
        /// Get the next matching callback for the <paramref name="url"/>
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The methodInfo callback and the optional parameters</returns>
        public Tuple<UrlNode, List<Object>> GetCallback(String url, String webMethod = "GET")
        {

            #region What should happen with ? params???

            if (url.IndexOf('?') > 0)
            {
                url = url.Substring(0, url.IndexOf('?'));
            }
            #endregion

            #region The WCF rest version seems to replace al // with / - so do the same

            //url = url.Replace("//", "/");

            #endregion

            if (!_RootNodes.ContainsKey(webMethod)) // try to find the method
            {
                return null;
            }
        
            var urlParts = url.Split(_Separators); // skip the first one because this is all in front of the first "/" and this is odd

            return GetCallback(_RootNodes[webMethod], urlParts, new List<Object>(), url, 0);

        }

        /// <summary>
        /// This will check
        /// </summary>
        /// <param name="urlNodes"></param>
        /// <param name="urlParts">the splittet</param>
        /// <param name="parameters">The parameters, parsed from the url</param>
        /// <param name="url">The uriginal url - this is needed to get the real parameter before splitting. Due to the split we lost the seperator char</param>
        /// <param name="posInUrl">The current pos in the url - this is needed to get the real parameter before splitting. Due to the split we lost the seperator char</param>
        /// <returns></returns>
        private Tuple<UrlNode, List<Object>> GetCallback(Dictionary<String, UrlNode> urlNodes, IEnumerable<String> urlParts, List<Object> parameters, String url, Int32 posInUrl)
        {

            var val = urlParts.FirstOrDefault();

            if (val == null)
            {
                throw new ArgumentNullException("urlParts.First");
            }
            val = val.ToLower();

            UrlNode curUrlNode = null;
            if (urlNodes.ContainsKey(val))
            {

                #region The current url node has this part of the URL

                curUrlNode = urlNodes[val];
                posInUrl += (val.Length == 0) ? 1 : val.Length + 1; // for just a / (which is now a empty string because of the split)

                #endregion

            }
            else if (urlNodes.ContainsKey("{}"))
            {

                #region If there is a wildcard in this node, use this

                curUrlNode = urlNodes["{}"];
                if (curUrlNode.ChildNodes == null || curUrlNode.ChildNodes.Count == 0)
                {
                    // this is the last parameter - so add the rest of the url as well
                    parameters.Add(url.Substring(posInUrl));
                    posInUrl = url.Length;
                    return new Tuple<UrlNode, List<object>>(curUrlNode, parameters);
                }
                else
                {
                    // just add this part and proceed
                    if (url.Length > posInUrl)
                    {
                        //parameters.Add(url.Substring(posInUrl, (val.Length == 0) ? 1 : val.Length));
                        parameters.Add(url.Substring(posInUrl, val.Length));
                        posInUrl += val.Length + 1; // add 1 for the missing seperator char
                    }
                    else
                    {
                        parameters.Add("");
                    }
                }

                #endregion

            }
            else
            {

                #region The node does not have this part and a wildcard neither

                return null;

                #endregion

            }


            if (urlParts.CountIsGreater(1) && !curUrlNode.ChildNodes.IsNullOrEmpty())
            {
                
                #region There are more url parts AND childs in the current node

                // we have some more childs defined
                Tuple<UrlNode, List<object>> retval = null;
                var newParams = new List<Object>();
                do
                {

                    #region As long as we can go deeper lets do it

                    retval = GetCallback(curUrlNode.ChildNodes, urlParts.Skip(1), newParams, url, posInUrl);
                    if (retval == null)
                    {

                        #region There is no hit for the current nodes childs and the next url parts

                        if (curUrlNode.ChildNodes.ContainsKey("{}"))
                        {

                            #region But the childs contains a wildcard we could use

                            curUrlNode = curUrlNode.ChildNodes["{}"];
                            if (curUrlNode.ChildNodes.IsNullOrEmpty())
                            {

                                #region The wildcard child has no more childs to verify, so lets take it

                                parameters.Add(url.Substring(posInUrl));
                                retval = new Tuple<UrlNode, List<object>>(curUrlNode, newParams);//parameters);

                                #endregion

                            }
                            else
                            {

                                #region The wildcard child have mor childs which needs to be verified

                                urlParts = urlParts.Skip(1);
                                if (GetCallback(curUrlNode.ChildNodes, urlParts.Skip(1), newParams, url, posInUrl) == null)
                                {

                                    #region The next parts do not leed into a successfull mapping, lets use this wildcard

                                    parameters.Add(url.Substring(posInUrl));
                                    retval = new Tuple<UrlNode, List<object>>(curUrlNode, parameters);
                                    parameters = null;

                                    #endregion

                                }
                                else
                                {

                                    #region Take this wildcard as parameter and proceed

                                    val = urlParts.First();
                                    newParams.Add(url.Substring(posInUrl, (val.Length == 0) ? 1 : val.Length));
                                    posInUrl += (val.Length == 0) ? 1 : val.Length + 1;

                                    #endregion

                                }

                                #endregion

                            }

                            #endregion

                        }
                        else
                        {

                            #region This part is still not valid, return null to proceed with the predecessor level

                            return null;

                            #endregion

                        }

                        #endregion

                    }

                    #endregion

                } while (retval == null);

                #region Are there any parameters to add to the result?

                if (!parameters.IsNullOrEmpty())
                {
                    #region We need to swap the parameters due to recursive call

                    parameters.AddRange(retval.Item2);
                    retval = new Tuple<UrlNode, List<object>>(retval.Item1, parameters);

                    #endregion
                }

                #endregion

                return retval;

                #endregion

            }


            else if (curUrlNode.Callback == null && !curUrlNode.ChildNodes.IsNullOrEmpty() && curUrlNode.ChildNodes.ContainsKey("{}"))
            {

                #region The current callback is null AND this is the last part of the url

                parameters.Add(url.Substring(posInUrl));
                curUrlNode = curUrlNode.ChildNodes["{}"];

                #endregion

            }

            else if (urlParts.CountIsGreater(1) && curUrlNode.ChildNodes.IsNullOrEmpty())
            {

                #region No childs but still some url parts

                return null;

                #endregion

            }

            else if (curUrlNode.Callback == null && !curUrlNode.ChildNodes.IsNullOrEmpty() && !curUrlNode.ChildNodes.ContainsKey("{}"))
            {

                #region There are childs but they have no placeholders ({}), so we have no valid definition

                return null;

                #endregion

            }

            return new Tuple<UrlNode, List<object>>(curUrlNode, parameters);

        }

        #endregion

    }
}
