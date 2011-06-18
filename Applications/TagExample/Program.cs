using System;
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
    public class Program
    {
        static void Main(string[] args)
        {
            #region initialize the DB

            //Make a new GraphDB instance
            IGraphDB GraphDB = new SonesGraphDB();

            IGraphQL GraphQL = new SonesQueryLanguage(GraphDB);

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
        private void CheckResult(QueryResult myQueryResult)
        {
            Console.WriteLine("{0} => {1}",
                                myQueryResult.Query,
                                myQueryResult.TypeOfResult);
        }

        public void Run(IGraphDB GraphDB, IGraphQL GraphQL, SecurityToken SecToken, TransactionToken TransToken)
        {
            #region create some types using the API

            #region create type Tag

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

            //create the type "Tag"
            var Tag = GraphDB.CreateVertexType<IVertexType>(SecToken, TransToken, new RequestCreateVertexType(Tag_VertexTypePredefinition), (Statistics, VertexType) => VertexType);

            #endregion

            #region insert some Tags

            var good = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                .AddStructuredProperty("Name", "good"), (Statistics, Result) => Result);

            var funny = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                .AddStructuredProperty("Name", "funny"), (Statistics, Result) => Result);

            #endregion

            #region create type Website

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

            Website_VertexTypePredefinition.AddOutgoingEdge(new OutgoingEdgePredefinition("Tags")
                                                                .SetAttributeType("Tag")
                                                                .SetMultiplicityAsMultiEdge());

            //create a index on type "Website" on property "Name"
            var MyIndex = new IndexPredefinition("MyIndex").SetIndexType("MultipleValuePersistent").AddProperty("Name").SetVertexType("Website");

            //add index
            Website_VertexTypePredefinition.AddIndex((IndexPredefinition)MyIndex);

            //add IncomingEdge "Tags", the related OutgoingEdge is "TaggedWebsites" on type "Tag"
            Website_VertexTypePredefinition.AddIncomingEdge(new IncomingEdgePredefinition("Tags")
                                                                .SetOutgoingEdge("Tag", "TaggedWebsites"));

            //create the type "Website"
            var Website = GraphDB.CreateVertexType<IVertexType>(SecToken, TransToken, new RequestCreateVertexType(Website_VertexTypePredefinition), (Statistics, VertexType) => VertexType);

            #endregion

            #region insert some Websites

            var cnn = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "CNN")
                                                                        .AddStructuredProperty("URL", "http://cnn.com/")
                                                                        .AddEdge(new EdgePredefinition("Tags")
                                                                                    .AddVertexID(Tag.ID, good.VertexID)), (Statistics, Result) => Result);

            var xkcd = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "xkcd")
                                                                        .AddStructuredProperty("URL", "http://xkcd.com/")
                                                                        .AddEdge(new EdgePredefinition("Tags")
                                                                                    .AddVertexID(Tag.ID, good.VertexID)
                                                                                    .AddVertexID(Tag.ID, funny.VertexID)), (Statistics, Result) => Result);

            var onion = GraphDB.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                        .AddStructuredProperty("Name", "onion")
                                                                        .AddStructuredProperty("URL", "http://theonion.com/")
                                                                        .AddEdge(new EdgePredefinition("Tags")
                                                                                    .AddVertexID(Tag.ID, funny.VertexID)), (Statistics, Result) => Result);

            #endregion

            #region make some SELECTS

            // Find out which tags xkcd is tagged with
            QueryResult _xkcdtags = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Tags WHERE w.Name = 'xkcd' DEPTH 1");

            CheckResult(_xkcdtags);
            
            foreach (var _tag in _xkcdtags.Vertices)
                Console.WriteLine(_tag.GetSingleEdge("Tags").GetTargetVertex().GetPropertyAsString("Name"));

            // List tagged sites
            var _taggedsites = GraphQL.Query(SecToken, TransToken, "FROM Website w SELECT w.Name, Count(w.Tags) AS Counter WHERE Count(w.Tags) > 0");

            CheckResult(_taggedsites);

            foreach (var _sites in _taggedsites.Vertices)
                Console.WriteLine("{0} => {1}", _sites.GetPropertyAsString("Name"), _sites.GetPropertyAsString("Counter"));

            #endregion

            #endregion

            GraphDB.Clear<IRequestStatistics>(SecToken, TransToken, new RequestClear(), (Statistics, DeletedTypes) => Statistics);

            #region create some types using the SonesQueryLanguage

            var Types = GraphQL.Query(SecToken, TransToken, @"CREATE VERTEX TYPES Tag ATTRIBUTES (String Name, SET<Websites> TaggedWebsites) INDICES (Name), 
                                                                                    Website ATTRIBUTES (String Name, String URL, SET<Tag> Tags)");

            var goodResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Tag VALUES (Name = 'good')");

            var funnyResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Tag VALUES (Name = 'funny')");

            var cnnResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'CNN', URL = 'http://cnn.com/', Tags = SETOF(Name = 'good'))");

            var xkcdResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'xkcd', URL = 'http://xkcd.com/', Tags = SETOF(Name = 'good', Name = 'funny'))");

            var onionResult = GraphQL.Query(SecToken, TransToken, "INSERT INTO Website VALUES (Name = 'onion', URL = 'http://theonion.com/', Tags = SETOF(Name = 'funny'))");

            #endregion
        }
    }
}
