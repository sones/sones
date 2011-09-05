using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphDS;
using sones.GraphDSServer;
using sones.GraphQL;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.Result;

/// <summary>
/// This is an Example wich describes and shows the simplicity of setting up a GraphDB by using the sones GraphDB CommunityEdition.
/// It shows you how to create your own Database by using the sones GraphDB C# API (using GraphDB Requests and the SonesQueryLanguage).
/// 
/// Please note the wiki Tutorial for further infomrations and descriptions under 
///     --> http://developers.sones.de/wiki/doku.php?id=tutorials:tagexample
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
namespace TagExample
{
    public class TagExample
    {
        #region public DATA

        //GraphDS server instance
        IGraphDSServer GraphDSServer;

        //Security- and TransactionToken
        SecurityToken SecToken;
        Int64 TransationID;

        #endregion

        #region constructor

        public TagExample()
        {
            //Make a new GraphDB instance
            var graphDB = new SonesGraphDB();
            
            var credentials = new UserPasswordCredentials("User", "test");

            //GraphDSServer = new GraphDS_Server(GraphDB, (ushort)9975, "User", "test", IPAddress.Any, PluginsAndParameters);
            GraphDSServer = new GraphDS_Server(graphDB, null);
            GraphDSServer.LogOn(credentials);
            //GraphDSServer.StartRESTService("", Properties.Settings.Default.ListeningPort, IPAddress.Any);

            //get a SecurityToken and an TransactionID
            SecToken = GraphDSServer.LogOn(credentials);
            TransationID = GraphDSServer.BeginTransaction(SecToken);
        }

        #endregion

        static void Main(string[] args)
        {
            //creating a new example instance
            var MyTagExample = new TagExample();

            //call the Run() method
            MyTagExample.Run();

            //Shutdown
            MyTagExample.Shutdown();
        }

        /// <summary>
        /// Shutdown of the TagExample
        /// </summary>
        public void Shutdown()
        {
            GraphDSServer.CommitTransaction(SecToken, TransationID);
            GraphDSServer.Shutdown(SecToken);
        }

        /// <summary>
        /// Starts the example, including creation of types "Tag" and "Website", insert some data and make some selects
        /// </summary>
        public void Run()
        {
            #region create types, create instances and additional work using the GraphDB API

            GraphDBRequests();

            #endregion
            
            //clear the DB (delete all created types) to create them again using the QueryLanguage
            GraphDSServer.Clear<IRequestStatistics>(SecToken, TransationID, new RequestClear(), (Statistics, DeletedTypes) => Statistics);

            #region create some types and insert values using the SonesQueryLanguage

            GraphQLQueries();

            #endregion

            #region make some SELECTS

            SELECTS();

            #endregion

            Console.WriteLine();
            Console.WriteLine("Finished Example. Type a key to finish!");
            Console.ReadKey();
        }

        #region private helper

        /// <summary>
        /// Describes how to define a type with user defined properties and indices and create some instances by using GraphDB requests.
        /// </summary>
        private void GraphDBRequests()
        {
            #region define type "Tag"

            //create a VertexTypePredefinition
            var Tag_VertexTypePredefinition = new VertexTypePredefinition("Tag");
            
            //create property
            var PropertyName = new PropertyPredefinition("Name", "String")
                                           .SetComment("This is a property on type 'Tag' named 'Name' and is of type 'String'");

            //add property
            Tag_VertexTypePredefinition.AddProperty(PropertyName);

            //create outgoing edge to "Website"
            var OutgoingEdgesTaggedWebsites = new OutgoingEdgePredefinition("TaggedWebsites", "Website")
                                                          .SetMultiplicityAsMultiEdge()
                                                          .SetComment(@"This is an outgoing edge on type 'Tag' wich points to the type 'Website' (the AttributeType) 
                                                                            and is defined as 'MultiEdge', which means that this edge can contain multiple single edges");

            //add outgoing edge
            Tag_VertexTypePredefinition.AddOutgoingEdge(OutgoingEdgesTaggedWebsites);

            #endregion

            #region define type "Website"

            //create a VertexTypePredefinition
            var Website_VertexTypePredefinition = new VertexTypePredefinition("Website");

            //create properties
            PropertyName = new PropertyPredefinition("Name", "String")
                                       .SetComment("This is a property on type 'Website' named 'Name' and is of type 'String'");

            var PropertyUrl = new PropertyPredefinition("URL", "String")
                                         .SetAsMandatory();

            //add properties
            Website_VertexTypePredefinition.AddProperty(PropertyName);
            Website_VertexTypePredefinition.AddProperty(PropertyUrl);

            #region create an index on type "Website" on property "Name"
            //there are three ways to set an index on property "Name" 
            //Beware: Use just one of them!

            //1. create an index definition and specifie the property- and type name
            var MyIndex = new IndexPredefinition("MyIndex").SetIndexType("SonesIndex").AddProperty("Name").SetVertexType("Website");
            //add index
            Website_VertexTypePredefinition.AddIndex((IndexPredefinition)MyIndex);

            //2. on creating the property definition of property "Name" call the SetAsIndexed() method, the GraphDB will create the index
            //PropertyName = new PropertyPredefinition("Name")
            //                           .SetAttributeType("String")
            //                           .SetComment("This is a property on type 'Website' with name 'Name' and is of type 'String'")
            //                           .SetAsIndexed();

            //3. make a create index request, like creating a type
            //BEWARE: This statement must be execute AFTER the type "Website" is created.
            //var MyIndex = GraphDSServer.CreateIndex<IIndexDefinition>(SecToken,
            //                                                          TransToken,
            //                                                          new RequestCreateIndex(
            //                                                          new IndexPredefinition("MyIndex")
            //                                                                   .SetIndexType("SonesIndex")
            //                                                                   .AddProperty("Name")
            //                                                                   .SetVertexType("Website")), (Statistics, Index) => Index);

            #endregion

            //add IncomingEdge "Tags", the related OutgoingEdge is "TaggedWebsites" on type "Tag"
            Website_VertexTypePredefinition.AddIncomingEdge(new IncomingEdgePredefinition("Tags",
                                                                                            "Tag", 
                                                                                            "TaggedWebsites"));

            #endregion
          
            
            #region create types by sending requests

            //create the types "Tag" and "Website"
            var DBTypes = GraphDSServer.CreateVertexTypes<IEnumerable<IVertexType>>(SecToken,
                                                                                    TransationID,
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
             * var Website = GraphDSServer.CreateVertexType<IVertexType>(SecToken, 
             *                                                           TransToken, 
             *                                                           new RequestCreateVertexType(Website_VertexTypePredefinition), 
             *                                                           (Statistics, VertexType) => VertexType);
             * 
             * //create the type "Tag"
             * var Tag = GraphDSServer.CreateVertexType<IVertexType>(SecToken, 
             *                                                       TransToken, 
             *                                                       new RequestCreateVertexType(Tag_VertexTypePredefinition), 
             *                                                       (Statistics, VertexType) => VertexType);
             */
            
            var Tag = DBTypes.Where(type => type.Name == "Tag").FirstOrDefault();

            var Website = DBTypes.Where(type => type.Name == "Website").FirstOrDefault();
           
            #endregion

            #region insert some Websites by sending requests

            var cnn = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "CNN")
                                                                                    .AddStructuredProperty("URL", "http://cnn.com/"),
                                                                                    (Statistics, Result) => Result);

            var xkcd = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "xkcd")
                                                                                    .AddStructuredProperty("URL", "http://xkcd.com/"),
                                                                                    (Statistics, Result) => Result);

            var onion = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "onion")
                                                                                    .AddStructuredProperty("URL", "http://theonion.com/"),
                                                                                    (Statistics, Result) => Result);

            //adding an unknown property means the property isn't defined before
            var test = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "Test")
                                                                                    .AddStructuredProperty("URL", "")
                                                                                    .AddUnknownProperty("Unknown", "unknown property"),
                                                                                    (Statistics, Result) => Result);

            #endregion

            #region insert some Tags by sending requests

            //insert a "Tag" with an OutgoingEdge to a "Website" include that the GraphDB creates an IncomingEdge on the given Website instances
            //(because we created an IncomingEdge on type "Website") --> as a consequence we never have to set any IncomingEdge
            var good = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Tag")
                                                                                    .AddStructuredProperty("Name", "good")
                                                                                    .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                        .AddVertexID(Website.ID, cnn.VertexID)
                                                                                        .AddVertexID(Website.ID, xkcd.VertexID)),
                                                                                    (Statistics, Result) => Result);

            var funny = GraphDSServer.Insert<IVertex>(SecToken, TransationID, new RequestInsertVertex("Tag")
                                                                                    .AddStructuredProperty("Name", "funny")
                                                                                    .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                        .AddVertexID(Website.ID, xkcd.VertexID)
                                                                                        .AddVertexID(Website.ID, onion.VertexID)),
                                                                                    (Statistics, Result) => Result);

            #endregion
            
            #region how to get a type from the DB, properties of the type, instances of a specific type and read out property values

            //how to get a type from the DB
            var TagDBType = GraphDSServer.GetVertexType<IVertexType>(SecToken, TransationID, new RequestGetVertexType(Tag.ID), (Statistics, Type) => Type);

            //read informations from type
            var typeName = TagDBType.Name;
            //are there other types wich extend the type "Tag"
            var hasChildTypes = TagDBType.HasChildTypes;
            //get the definition of the property "Name"
            var propName = TagDBType.GetPropertyDefinition("Name");

            //how to get all instances of a type from the DB
            var TagInstances = GraphDSServer.GetVertices(SecToken, TransationID, new RequestGetVertices(TagDBType.ID), (Statistics, Vertices) => Vertices);

            foreach (var item in TagInstances)
            {
                //to get the value of a property of an instance, you need the property ID 
                //(that's why we fetched the type from DB an read out the property definition of property "Name")
                var name = item.GetPropertyAsString(propName.ID);
            }

            #endregion
        }

        /// <summary>
        /// Describes how to send queries using the GraphQL.
        /// </summary>
        private void GraphQLQueries()
        {
            #region create types
            //create types at the same time, because of the circular dependencies (Tag has OutgoingEdge to Website, Website has IncomingEdge from Tag)
            //like shown before, using the GraphQL there are also three different ways to create create an index on property "Name" of type "Website"
            //1. create an index definition and specifie the property name and index type
            var Types = GraphDSServer.Query(SecToken, TransationID, @"CREATE VERTEX TYPES Tag ATTRIBUTES (String Name, SET<Website> TaggedWebsites), 
                                                                                Website ATTRIBUTES (String Name, String URL) INCOMINGEDGES (Tag.TaggedWebsites Tags) 
                                                                                    INDICES (MyIndex INDEXTYPE SonesIndex ON ATTRIBUTES Name)", SonesGQLConstants.GQL);

            //2. on creating the type with the property "Name", just define the property "Name" under INDICES
            //var Types = GraphQL.Query(SecToken, TransToken, @"CREATE VERTEX TYPES Tag ATTRIBUTES (String Name, SET<Website> TaggedWebsites), 
            //                                                                    Website ATTRIBUTES (String Name, String URL) INCOMINGEDGES (Tag.TaggedWebsites Tags) INDICES (Name)");

            //3. make a create index query
            //var Types = GraphQL.Query(SecToken, TransToken, @"CREATE VERTEX TYPES Tag ATTRIBUTES (String Name, SET<Website> TaggedWebsites), 
            //                                                                    Website ATTRIBUTES (String Name, String URL) INCOMINGEDGES (Tag.TaggedWebsites Tags)");
            //var MyIndex = GraphQL.Query(SecToken, TransToken, "CREATE INDEX MyIndex ON VERTEX TYPE Website (Name) INDEXTYPE SonesIndex");            
            CheckResult(Types);
            #endregion

            #region create instances of type "Website"

            var cnnResult = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Website VALUES (Name = 'CNN', URL = 'http://cnn.com/')", SonesGQLConstants.GQL);
            CheckResult(cnnResult);

            var xkcdResult = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Website VALUES (Name = 'xkcd', URL = 'http://xkcd.com/')", SonesGQLConstants.GQL);
            CheckResult(xkcdResult);

            var onionResult = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Website VALUES (Name = 'onion', URL = 'http://theonion.com/')", SonesGQLConstants.GQL);
            CheckResult(onionResult);

            //adding an unknown property ("Unknown") means the property isn't defined before
            var unknown = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Website VALUES (Name = 'Test', URL = '', Unknown = 'unknown property')", SonesGQLConstants.GQL);
            CheckResult(onionResult);

            #endregion

            #region create instances of type "Tag"

            var goodResult = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Tag VALUES (Name = 'good', TaggedWebsites = SETOF(Name = 'CNN', Name = 'xkcd'))", SonesGQLConstants.GQL);
            CheckResult(goodResult);

            var funnyResult = GraphDSServer.Query(SecToken, TransationID, "INSERT INTO Tag VALUES (Name = 'funny', TaggedWebsites = SETOF(Name = 'xkcd', Name = 'onion'))", SonesGQLConstants.GQL);
            CheckResult(funnyResult);

            #endregion
        }

        /// <summary>
        /// Executes some select statements.
        /// </summary>
        private void SELECTS()
        {
            // find out which tags xkcd is tagged with
            var _xkcdtags = GraphDSServer.Query(SecToken, TransationID, "FROM Website w SELECT w.Tags WHERE w.Name = 'xkcd' DEPTH 1", SonesGQLConstants.GQL);

            CheckResult(_xkcdtags);

            foreach (var _tag in _xkcdtags.Vertices)
                foreach (var edge in _tag.GetHyperEdge("Tags").GetAllEdges())
                    Console.WriteLine(edge.GetTargetVertex().GetPropertyAsString("Name"));

            // List tagged sites names and the count of there tags
            var _taggedsites = GraphDSServer.Query(SecToken, TransationID, "FROM Website w SELECT w.Name, w.Tags.Count() AS Counter", SonesGQLConstants.GQL);

            CheckResult(_taggedsites);

            foreach (var _sites in _taggedsites.Vertices)
                Console.WriteLine("{0} => {1}", _sites.GetPropertyAsString("Name"), _sites.GetPropertyAsString("Counter"));

            // find out the URL's of the website of each Tag
            var _urls = GraphDSServer.Query(SecToken, TransationID, "FROM Tag t SELECT t.Name, t.TaggedWebsites.URL", SonesGQLConstants.GQL);

            CheckResult(_urls);

            foreach (var _tag in _urls.Vertices)
                foreach (var edge in _tag.GetHyperEdge("TaggedWebsites").GetAllEdges())
                    Console.WriteLine(_tag.GetPropertyAsString("Name") + " - " + edge.GetTargetVertex().GetPropertyAsString("URL"));
        }

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
