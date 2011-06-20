using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Request.CreateVertexTypes;
using sones.GraphDB.TypeSystem;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;

namespace TagExample
{
    /// <summary>
    /// This is a Example wich describes and shows the simplicity of setting up a GraphDB by using the sones GraphDB CommunityEdition.
    /// It shows you how to create our own Database by using different sones GraphDB API's (using GraphDB Requests and the SonesQueryLanguage).
    /// 
    /// If you are using the SonesQueryLanguage please read our GQL CheatSheet 
    ///     --> https://github.com/downloads/sones/sones/GQL_cheatsheet_latest.pdf
    /// there you can find the description of all available statements and some additional examples.
    /// 
    /// In this Example we show how to:
    ///     - create user defined types, add structured properties, add unknown properties
    ///     - create outgoing and incoming edges on a type
    ///     - create an index on a specified property in 3 different ways
    ///     - set constraints on properties (like "unique" and "mandatory")
    ///     
    ///     - set up queries and analyse them
    ///     - use Function and Aggregates in a query
    /// </summary>
    public class Example
    {

        static void Main(string[] args)
        {
            #region initialize the DB

            //Make a new GraphDB instance
            IGraphDB GraphDB = new SonesGraphDB();
            //Make a new SonesQueryLanguage instance (GQL)
            IGraphQL GraphQL = new SonesQueryLanguage(GraphDB);

            //get a Security- and TransactionToken
            SecurityToken SecToken = GraphDB.LogOn(new UserPasswordCredentials("root", "1111"));
            TransactionToken TransToken = GraphDB.BeginTransaction(SecToken);
            
            #endregion

            var MyTagExample = new TagExample();

            MyTagExample.Run(GraphDB, GraphQL, SecToken, TransToken);

            //shutdown GraphDB
            GraphDB.Shutdown(SecToken);
        }
        
    }

    public class TagExample
    {
        #region constructor

        public TagExample()
        { }

        #endregion

        /// <summary>
        /// Starts the example, including creation of types "Tag" and "Website", insert some data and make some selects
        /// </summary>
        /// <param name="GraphDB">The GraphDB instance.</param>
        /// <param name="GraphQL">The QueryLanguage instance.</param>
        /// <param name="SecToken">The SecurityToken.</param>
        /// <param name="TransToken">The Transaction Token.</param>
        public void Run(IGraphDB GraphDB, IGraphQL GraphQL, SecurityToken SecToken, TransactionToken TransToken)
        {
            #region create some types using the API

            #region define type "Tag"

            //create a VertexTypePredefinition
            var Tag_VertexTypePredefinition = new VertexTypePredefinition("Tag");

            //create property
            var PropertyName = new PropertyPredefinition("Name")
                                    .SetAttributeType("String")
                                    .SetComment("This is a property on type 'Tag' with name 'Name' and is of type 'String'");

            //add property
            Tag_VertexTypePredefinition.AddProperty(PropertyName);

            //create outgoing edge to "Tag"
            var OutgoingEdgesTaggedWebsites = new OutgoingEdgePredefinition("TaggedWebsites")
                                                    .SetAttributeType("Website")
                                                    .SetMultiplicityAsMultiEdge();

            //add outgoing edge
            Tag_VertexTypePredefinition.AddOutgoingEdge(OutgoingEdgesTaggedWebsites);

            #endregion

            #region define type "Website"

            //create a VertexTypePredefinition
            var Website_VertexTypePredefinition = new VertexTypePredefinition("Website");

            //create properties
            PropertyName = new PropertyPredefinition("Name")
                                .SetAttributeType("String")
                                .SetComment("This is a property on type 'Website' with name 'Name' and is of type 'String'");

            var PropertyUrl = new PropertyPredefinition("URL")
                                    .SetAttributeType("String")
                                    .SetAsMandatory();

            //add properties
            Website_VertexTypePredefinition.AddProperty(PropertyName);
            Website_VertexTypePredefinition.AddProperty(PropertyUrl);

            #region create an index on type "Website" on property "Name"
            //there are three ways to set an index on property "Name" 
            //Beware: Use just one of them!

            //1. create an index definition and specifie the property- and type name
            var MyIndex = new IndexPredefinition("MyIndex").SetIndexType("MultipleValueIndex").AddProperty("Name").SetVertexType("Website");
            //add index
            Website_VertexTypePredefinition.AddIndex((IndexPredefinition)MyIndex);

            //2. on creating the property definition of property "Name" call the SetAsIndexed() method, the GraphDB will create the index
            //PropertyName = new PropertyPredefinition("Name")
            //                    .SetAttributeType("String")
            //                    .SetComment("This is a property on type 'Website' with name 'Name' and is of type 'String'")
            //                    .SetAsIndexed();

            //3. make a create index request, like creating a type
            //var MyIndex = GraphDB.CreateIndex<IIndexDefinition>(SecToken, 
            //                                                    TransToken, 
            //                                                    new RequestCreateIndex(
            //                                                        new IndexPredefinition("MyIndex")
            //                                                            .SetIndexType("MultipleValueIndex")
            //                                                            .AddProperty("Name")
            //                                                            .SetVertexType("Website")), (Statistics, Index) => Index);

            #endregion

            //add IncomingEdge "Tags", the related OutgoingEdge is "TaggedWebsites" on type "Tag"
            Website_VertexTypePredefinition.AddIncomingEdge(new IncomingEdgePredefinition("Tags")
                                                                .SetOutgoingEdge("Tag", "TaggedWebsites"));

            #endregion

            #region create types by sending requests

            //create the types "Tag" and "Website"
            var DBTypes = GraphDB.CreateVertexTypes<IEnumerable<IVertexType>>(SecToken, 
                                                                                TransToken, 
                                                                                new RequestCreateVertexTypes(
                                                                                    new List<VertexTypePredefinition> { Tag_VertexTypePredefinition, 
                                                                                                                        Website_VertexTypePredefinition }), 
                                                                                (Statistics, VertexTypes) => VertexTypes);

            /* 
             * BEWARE: The following two operations won't work because the two types "Tag" and "Website" depending on each other,
             *          because one type has an incoming edge to the other and the other one has an incoming edge, 
             *          so they cannot be created separate (by using create type),
             *          they have to be created at the same time (by using create types)
             *          
             * //create the type "Website"
             * var Website = GraphDB.CreateVertexType<IVertexType>(SecToken, 
             *                                                      TransToken, 
             *                                                      new RequestCreateVertexType(Website_VertexTypePredefinition), 
             *                                                      (Statistics, VertexType) => VertexType);
             * 
             * //create the type "Tag"
             * var Tag = GraphDB.CreateVertexType<IVertexType>(SecToken, 
             *                                                  TransToken, 
             *                                                  new RequestCreateVertexType(Tag_VertexTypePredefinition), 
             *                                                  (Statistics, VertexType) => VertexType);
             */

            var Tag = DBTypes.Where(type => type.Name == "Tag").FirstOrDefault();

            var Website = DBTypes.Where(type => type.Name == "Website").FirstOrDefault();

            #endregion

            #region insert some Websites by sending requests

            var cnn = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "CNN")
                                                                        .AddStructuredProperty("URL", "http://cnn.com/"), 
                                                                        (Statistics, Result) => Result);

            var xkcd = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "xkcd")
                                                                        .AddStructuredProperty("URL", "http://xkcd.com/"),
                                                                        (Statistics, Result) => Result);

            var onion = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "onion")
                                                                        .AddStructuredProperty("URL", "http://theonion.com/"), 
                                                                        (Statistics, Result) => Result);

            //adding an unknown property means the property isn't defined before
            var test = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "Test")
                                                                        .AddStructuredProperty("URL", "") 
                                                                        .AddUnknownProperty("Unknown", "unknown property"),
                                                                        (Statistics, Result) => Result);

            #endregion

            #region insert some Tags by sending requests

            //insert a "Tag" with an OutgoingEdge to a "Website" include that the GraphDB creates an IncomingEdge on the given Website instances
            //(because we created an IncomingEdge on type "Website") --> as a consequence we never have to set any IncomingEdge
            var good = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                .AddStructuredProperty("Name", "good")
                                                                                .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                    .AddVertexID(Website.ID, cnn.VertexID)
                                                                                    .AddVertexID(Website.ID, xkcd.VertexID)), 
                                                                                (Statistics, Result) => Result);

            var funny = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                .AddStructuredProperty("Name", "funny")
                                                                                .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                    .AddVertexID(Website.ID, xkcd.VertexID)
                                                                                    .AddVertexID(Website.ID, onion.VertexID)), 
                                                                                (Statistics, Result) => Result);

            #endregion

            #region make some SELECTS

            // Find out which tags xkcd is tagged with
            QueryResult _xkcdtags = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Tags WHERE w.Name = 'xkcd' DEPTH 1");

            CheckResult(_xkcdtags);
            
            foreach (var _tag in _xkcdtags.Vertices)
                foreach (var edge in _tag.GetHyperEdge("Tags").GetAllEdges())
                    Console.WriteLine(edge.GetTargetVertex().GetPropertyAsString("Name"));

            // List tagged sites
            var _taggedsites = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Name, w.Tags.Count() AS Counter");

            CheckResult(_taggedsites);

            foreach (var _sites in _taggedsites.Vertices)
                Console.WriteLine("{0} => {1}", _sites.GetPropertyAsString("Name"), _sites.GetPropertyAsString("Counter"));

            // find out the URL's of the website of each Tag
            var _urls = GraphQL.Query(SecToken, TransToken, "FROM Tag t SELECT t.Name, t.TaggedWebsites.URL");

            CheckResult(_urls);

            foreach (var _tag in _urls.Vertices)
                foreach (var edge in _tag.GetHyperEdge("TaggedWebsites").GetAllEdges())
                    Console.WriteLine(_tag.GetPropertyAsString("Name") + " - " + edge.GetTargetVertex().GetPropertyAsString("URL"));

            #endregion

            #endregion

            //clear the DB (delete all created types) to create them again using the QueryLanguage
            GraphDB.Clear<IRequestStatistics>(SecToken, TransToken, new RequestClear(), (Statistics, DeletedTypes) => Statistics);

            #region create some types and insert values using the SonesQueryLanguage

            var Types = GraphQL.Query(SecToken, TransToken, @"CREATE VERTEX TYPES Tag ATTRIBUTES (String Name, SET<Website> TaggedWebsites) INDICES (Name), 
                                                                                Website ATTRIBUTES (String Name, String URL) INCOMINGEDGES (Tag.TaggedWebsites Tags)");
            CheckResult(Types);

            var cnnResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'CNN', URL = 'http://cnn.com/')");
            CheckResult(cnnResult);

            var xkcdResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'xkcd', URL = 'http://xkcd.com/')");
            CheckResult(xkcdResult);

            var onionResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'onion', URL = 'http://theonion.com/')");
            CheckResult(onionResult);

            //adding an unknown property means the property isn't defined before
            var unknown = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'Test', URL = '', Unknown = 'unknown property')");
            CheckResult(onionResult);

            var goodResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Tag VALUES (Name = 'good', TaggedWebsites = SETOF(Name = 'CNN', Name = 'xkcd'))");
            CheckResult(goodResult);

            var funnyResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Tag VALUES (Name = 'funny', TaggedWebsites = SETOF(Name = 'xkcd', Name = 'onion'))");
            CheckResult(funnyResult);

            #endregion

            #region make some SELECTS

            // Find out which tags xkcd is tagged with
            _xkcdtags = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Tags WHERE w.Name = 'xkcd' DEPTH 1");

            CheckResult(_xkcdtags);

            foreach (var _tag in _xkcdtags.Vertices)
                foreach (var edge in _tag.GetHyperEdge("Tags").GetAllEdges())
                    Console.WriteLine(edge.GetTargetVertex().GetPropertyAsString("Name"));

            // List tagged sites
            _taggedsites = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Name, w.Tags.Count() AS Counter");

            CheckResult(_taggedsites);

            foreach (var _sites in _taggedsites.Vertices)
                Console.WriteLine("{0} => {1}", _sites.GetPropertyAsString("Name"), _sites.GetPropertyAsString("Counter"));

            // find out the URL's of the website of each Tag
            _urls = GraphQL.Query(SecToken, TransToken, "FROM Tag t SELECT t.Name, t.TaggedWebsites.URL");

            CheckResult(_urls);

            foreach (var _tag in _urls.Vertices)
                foreach (var edge in _tag.GetHyperEdge("TaggedWebsites").GetAllEdges())
                    Console.WriteLine(_tag.GetPropertyAsString("Name") + " - " + edge.GetTargetVertex().GetPropertyAsString("URL"));

            #endregion

            Console.WriteLine();

            Console.WriteLine("Finished Example. Type in a query OR \"exit\" for finish!");

            Console.WriteLine();

            #region read in queries
            
            var line = Console.ReadLine();

            while (line.ToUpper() != "EXIT")
            { 
                var temp = GraphQL.Query(SecToken, TransToken, line);

                if (CheckResult(temp))
                {
                    foreach (var item in temp.Vertices)
                    {
                        Console.WriteLine("Edges:");
                        foreach (var edge in item.GetAllEdges())
                            Console.WriteLine(edge.Item1 + " " + edge.Item2);
                        Console.WriteLine();
                        Console.WriteLine("Attributes:");
                        foreach (var attr in item.GetAllProperties())
                            Console.WriteLine(attr.Item1 + " " + attr.Item2);
                        Console.WriteLine();
                    }
                }

                line = Console.ReadLine();
            }

            #endregion
        }

        #region private helper

        /// <summary>
        /// This private method analyses the QueryResult, shows the ResultType and Errors if existing.
        /// </summary>
        /// <param name="myQueryResult">The result of a query.</param>
        private bool CheckResult(QueryResult myQueryResult)
        {
            if (myQueryResult.Error != null)
            {
                if (myQueryResult.Error.InnerException != null)
                    Console.WriteLine(myQueryResult.Error.InnerException.Message);
                else
                    Console.WriteLine(myQueryResult.Error.Message);

                return false;
            }
            else
            {
                Console.WriteLine("Query " + myQueryResult.TypeOfResult);

                return true;
            }
        }

        #endregion
    }
}
