/*
 * DBCLI_OM
 * (c) Henning Rauch, 2009
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphDB.Managers.Structures;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// This command enables the user to load and store 
    /// an ontology in OWL.
    /// </summary>

    public class DBCLI_OM : AllAdvancedDBCLICommands
    {

        #region Constructor

        public DBCLI_OM()
        {

            // Command name and description
            InitCommand("OM",
                        "Loads and stores an ontology",
                        "Loads and stores an ontology");


            #region Symbol declaration

            SymbolTerminal OM_CommandString = Symbol("OM");
            SymbolTerminal OM_ProtonString = Symbol("proton");

            #endregion

            #region Non-terminal declaration

            NonTerminal OM = new NonTerminal("OM");
            NonTerminal OM_Action = new NonTerminal("OM_Action");
            NonTerminal OM_Load_options = new NonTerminal("OM_Load_options");
            NonTerminal OM_Store_options = new NonTerminal("OM_Store_options");

            #endregion



            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + OM_Action);


            #region BNF rules

            OM.GraphOptions.Add(GraphOption.IsCommandRoot);

            OM_Action.Rule = StoreSymbol + OM_Store_options
                                | LoadSymbol + OM_Load_options;
            OM_Action.GraphOptions.Add(GraphOption.IsOption);

            OM_Load_options.Rule = OM_ProtonString
                                    | stringLiteralExternalEntry;
            OM_Load_options.GraphOptions.Add(GraphOption.IsOption);

            OM_Store_options.Rule = stringLiteralExternalEntry;
            OM_Store_options.GraphOptions.Add(GraphOption.IsOption);

            #endregion

            #region Non-terminal integration

            _CommandNonTerminals.Add(OM_Action);
            _CommandNonTerminals.Add(OM_Load_options);
            _CommandNonTerminals.Add(OM_Store_options);

            #endregion

            #region Symbol integration

            _CommandSymbolTerminal.Add(OM_ProtonString);

            #endregion

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            switch (myOptions.ElementAt(1).Value[0].Option)
            {

                case "load":
                    switch (myOptions.ElementAt(2).Value[0].Option)
                    {
                        case "proton":
                            LoadTheProtonOntology(myAGraphDSSharp.IGraphDBSession, myAGraphDSSharp);
                            break;

                        default:
                            WriteLine("Currently not implemented.");
                            break;
                    }
                    break;

                case "store":
                    WriteLine("Currently not implemented.");
                    break;

            }

            return Exceptional.OK;

        }

        private void LoadTheProtonOntology(IGraphDBSession myIGraphDBSession, Object myIGraphFS)
        {

            using (var _Transaction = myIGraphDBSession.BeginTransaction())
            {

                var dbContext = (DBContext)_Transaction.GetDBContext();

                #region Entity

                dbContext.DBTypeManager.AddType("Entity", DBConstants.DBBaseObject, new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Entity", "label", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("Entity", "comment", DBConstants.DBString);
                //dbContext.DBTypeManager.AddAttributeToType("Entity", "locatedIn", "Location");
                //dbContext.DBTypeManager.AddAttributeToType("Entity", "partOf", "Entity");


                #region Abstract

                dbContext.DBTypeManager.AddType("Abstract", "Entity", new Dictionary<AttributeDefinition, string>(), "");

                #region BusinessAbstraction

                dbContext.DBTypeManager.AddType("BusinessAbstraction", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region IndustrySector

                dbContext.DBTypeManager.AddType("IndustrySector", "BusinessAbstraction", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("IndustrySector", "subSectorOf", "IndustrySector");
                dbContext.DBTypeManager.AddAttributeToType("IndustrySector", "hasCode", DBConstants.DBString);

                #endregion

                #region Market

                dbContext.DBTypeManager.AddType("Market", "BusinessAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region UserAbstraction

                dbContext.DBTypeManager.AddType("UserAbstraction", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region KissNoFrogUser

                dbContext.DBTypeManager.AddType("KissNoFrogUser", "UserAbstraction", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasMeetingLowerAge", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasMeetingUpperAge", DBConstants.DBInteger);
                //dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasTags", "LIST<Tag>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasFavourites", "LIST<KissNoFrogUser>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasVisitors", "LIST<KissNoFrogUser>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasFriends", "LIST<KissNoFrogUser>");
                //dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasLeisure", "LIST<Leisure>");
                //dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasFulltxtLeisure", "LIST<FulltxtLeisure>");
                //dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "likeSex", "SexualOrientation");
                //dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasPerson", "Person");

                #region KissNoFrogVisitor

                dbContext.DBTypeManager.AddType("KissNoFrogVisitor", "KissNoFrogUser", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogVisitor", "whenVisited", DBConstants.DBDateTime);

                #endregion

                #endregion

                #endregion

                #region ContactInformation

                dbContext.DBTypeManager.AddType("ContactInformation", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region Adress

                dbContext.DBTypeManager.AddType("Adress", "ContactInformation", new Dictionary<AttributeDefinition, string>(), "");

                #region PostalAdress

                dbContext.DBTypeManager.AddType("PostalAdress", "Adress", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region InternetAdress

                dbContext.DBTypeManager.AddType("InternetAdress", "ContactInformation", new Dictionary<AttributeDefinition, string>(), "");

                #region EMail

                dbContext.DBTypeManager.AddType("EMail", "InternetAdress", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region IPAdress

                dbContext.DBTypeManager.AddType("IPAdress", "InternetAdress", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region InternetDomain

                dbContext.DBTypeManager.AddType("InternetDomain", "InternetAdress", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region WebPage

                dbContext.DBTypeManager.AddType("WebPage", "InternetAdress", new Dictionary<AttributeDefinition, string>(), "");

                #region HomePage

                dbContext.DBTypeManager.AddType("HomePage", "WebPage", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region PhoneNumber

                dbContext.DBTypeManager.AddType("PhoneNumber", "ContactInformation", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region GeneralTerm

                dbContext.DBTypeManager.AddType("GeneralTerm", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region Tag

                dbContext.DBTypeManager.AddType("Tag", "GeneralTerm", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Leisure

                dbContext.DBTypeManager.AddType("Leisure", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region FulltxtLeisure

                dbContext.DBTypeManager.AddType("FulltxtLeisure", "Leisure", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Language

                dbContext.DBTypeManager.AddType("Language", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region SocialAbstraction

                dbContext.DBTypeManager.AddType("SocialAbstraction", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region Art

                dbContext.DBTypeManager.AddType("Art", "SocialAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Money

                dbContext.DBTypeManager.AddType("Money", "SocialAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Profession

                dbContext.DBTypeManager.AddType("Profession", "SocialAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region ResearchArea

                dbContext.DBTypeManager.AddType("ResearchArea", "SocialAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Sport

                dbContext.DBTypeManager.AddType("Sport", "SocialAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region TemporalAbstraction

                dbContext.DBTypeManager.AddType("TemporalAbstraction", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region CalendarMonth

                dbContext.DBTypeManager.AddType("CalendarMonth", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region DayOfMonth

                dbContext.DBTypeManager.AddType("DayOfMonth", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region DayOfWeek

                dbContext.DBTypeManager.AddType("DayOfWeek", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region DayTime

                dbContext.DBTypeManager.AddType("DayTime", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Festival

                dbContext.DBTypeManager.AddType("Festival", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Season

                dbContext.DBTypeManager.AddType("Season", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region TimeZone

                dbContext.DBTypeManager.AddType("TimeZone", "TemporalAbstraction", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Topic

                dbContext.DBTypeManager.AddType("Topic", "Abstract", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Topic", "isSubTopicOf", "Topic");

                #endregion

                #region NaturalPhenomenon

                dbContext.DBTypeManager.AddType("NaturalPhenomenon", "Abstract", new Dictionary<AttributeDefinition, string>(), "");

                #region SexualOrientation

                dbContext.DBTypeManager.AddType("SexualOrientation", "NaturalPhenomenon", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Happening

                dbContext.DBTypeManager.AddType("Happening", "Entity", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Happening", "startTime", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Happening", "endTime", DBConstants.DBDateTime);

                #region Event

                dbContext.DBTypeManager.AddType("Event", "Happening", new Dictionary<AttributeDefinition, string>(), "");

                #region Accident

                dbContext.DBTypeManager.AddType("Accident", "Event", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region ArtPerformace

                dbContext.DBTypeManager.AddType("ArtPerformace", "Event", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Meeting

                dbContext.DBTypeManager.AddType("Meeting", "Event", new Dictionary<AttributeDefinition, string>(), "");

                #region KNFMeeting

                dbContext.DBTypeManager.AddType("KNFMeeting", "Meeting", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "ageLowerBound", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "ageUpperBound", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "isVisible", DBConstants.DBBoolean);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "hasParticipants", "LIST<KissNoFrogUser>");

                #endregion

                #region BoardMeeting

                dbContext.DBTypeManager.AddType("BoardMeeting", "Meeting", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region OfficialPoliticalMeeting

                dbContext.DBTypeManager.AddType("OfficialPoliticalMeeting", "Meeting", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region MilitaryConflict

                dbContext.DBTypeManager.AddType("MilitaryConflict", "Event", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Project

                dbContext.DBTypeManager.AddType("Project", "Event", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region SportEvent

                dbContext.DBTypeManager.AddType("SportEvent", "Event", new Dictionary<AttributeDefinition, string>(), "");

                #region OlympicGames

                dbContext.DBTypeManager.AddType("OlympicGames", "SportEvent", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region SportGame

                dbContext.DBTypeManager.AddType("SportGame", "SportEvent", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Tournament

                dbContext.DBTypeManager.AddType("Tournament", "SportEvent", new Dictionary<AttributeDefinition, string>(), "");

                #endregion


                #endregion

                #endregion

                #region Situation

                dbContext.DBTypeManager.AddType("Situation", "Happening", new Dictionary<AttributeDefinition, string>(), "");

                #region JobPosition

                dbContext.DBTypeManager.AddType("JobPosition", "Situation", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "withinOrganization", "Organization");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldFrom", "Person");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldto", "Person");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "holder", "Person");

                #region BoardMember

                dbContext.DBTypeManager.AddType("BoardMember", "JobPosition", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Employee

                dbContext.DBTypeManager.AddType("Employee", "JobPosition", new Dictionary<AttributeDefinition, string>(), "");

                #region Manager

                dbContext.DBTypeManager.AddType("Manager", "Employee", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Leader

                dbContext.DBTypeManager.AddType("Leader", "JobPosition", new Dictionary<AttributeDefinition, string>(), "");

                #region Chairman

                dbContext.DBTypeManager.AddType("Chairman", "Leader", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Executive

                dbContext.DBTypeManager.AddType("Executive", "Leader", new Dictionary<AttributeDefinition, string>(), "");

                #region CEO

                dbContext.DBTypeManager.AddType("CEO", "Executive", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Minister

                dbContext.DBTypeManager.AddType("Minister", "Executive", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Premier

                dbContext.DBTypeManager.AddType("Premier", "Executive", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region President

                dbContext.DBTypeManager.AddType("President", "Leader", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region MemberOfParliament

                dbContext.DBTypeManager.AddType("MemberOfParliament", "JobPosition", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region OfficialPosition

                dbContext.DBTypeManager.AddType("OfficialPosition", "JobPosition", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("OfficialPosition", "officialPositionIn", "Location");

                #endregion

                #endregion

                #region Role

                dbContext.DBTypeManager.AddType("Role", "Situation", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Role", "roleIn", "LIST<Happening>");
                //dbContext.DBTypeManager.AddAttributeToType("Role", "roleHolder", "LIST<Object>"); //loaded 

                #endregion

                #endregion

                #region TimeInterval

                dbContext.DBTypeManager.AddType("TimeInterval", "Happening", new Dictionary<AttributeDefinition, string>(), "");

                #region CalendarYear

                dbContext.DBTypeManager.AddType("CalendarYear", "TimeInterval", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Date

                dbContext.DBTypeManager.AddType("Date", "TimeInterval", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Month

                dbContext.DBTypeManager.AddType("Month", "TimeInterval", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Quarter

                dbContext.DBTypeManager.AddType("Quarter", "TimeInterval", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Week

                dbContext.DBTypeManager.AddType("Week", "TimeInterval", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Object

                dbContext.DBTypeManager.AddType("Object", "Entity", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Object", "hasContactInfo", "ContactInformation");
                //dbContext.DBTypeManager.AddAttributeToType("Object", "isOwnedBy", "Agent");

                #region Account

                dbContext.DBTypeManager.AddType("Account", "Object", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Account", "accountProvider", "Agent");

                #endregion

                #region Agent

                dbContext.DBTypeManager.AddType("Agent", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Agent", "involvedIn", "LIST<Happening>");
                dbContext.DBTypeManager.AddAttributeToType("Agent", "isLegalEntity", DBConstants.DBBoolean);
                dbContext.DBTypeManager.AddAttributeToType("Agent", "partiallyControls", "LIST<Object>");

                #region Group

                dbContext.DBTypeManager.AddType("Group", "Agent", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Group", "hasMember", "LIST<Agent>");

                #region Organization

                dbContext.DBTypeManager.AddType("Organization", "Group", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "childOrganizationOf", "Organization");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "doingBusinessAs", DBConstants.DBString);
                //dbContext.DBTypeManager.AddAttributeToType("Organization", "establishedIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "establishmentDate", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Organization", "numberOfEmployees", DBConstants.DBInteger);
                //dbContext.DBTypeManager.AddAttributeToType("Organization", "registeredIn", "Location");

                #region Charity

                dbContext.DBTypeManager.AddType("Charity", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region CommercialOrganization

                dbContext.DBTypeManager.AddType("CommercialOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("CommercialOrganization", "activeInSector", "IndustrySector");
                dbContext.DBTypeManager.AddAttributeToType("CommercialOrganization", "hasShareholder", "LIST<Agent>");

                #region Company

                dbContext.DBTypeManager.AddType("Company", "CommercialOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #region Airline

                dbContext.DBTypeManager.AddType("Airline", "Company", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region Bank

                dbContext.DBTypeManager.AddType("Bank", "Company", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region Telecom

                dbContext.DBTypeManager.AddType("Telecom", "Company", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #endregion

                #endregion

                #region Division

                dbContext.DBTypeManager.AddType("Division", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region EducationalOrganization

                dbContext.DBTypeManager.AddType("EducationalOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #region School

                dbContext.DBTypeManager.AddType("School", "EducationalOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region University

                dbContext.DBTypeManager.AddType("University", "EducationalOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region GovernmentOrganization

                dbContext.DBTypeManager.AddType("GovernmentOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("GovernmentOrganization", "ofCountry", "Country");

                #region Government

                dbContext.DBTypeManager.AddType("Government", "GovernmentOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Ministry

                dbContext.DBTypeManager.AddType("Ministry", "GovernmentOrganization", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Ministry", "hasMinister", "LIST<Person>");

                #endregion

                #endregion

                #region InternationalOrganization

                dbContext.DBTypeManager.AddType("InternationalOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region PoliticalEntity

                dbContext.DBTypeManager.AddType("PoliticalEntity", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #region Parliament

                dbContext.DBTypeManager.AddType("Parliament", "PoliticalEntity", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region PoliticalParty

                dbContext.DBTypeManager.AddType("PoliticalParty", "PoliticalEntity", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region ReligiousOrganization

                dbContext.DBTypeManager.AddType("ReligiousOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region ResearchOrganization

                dbContext.DBTypeManager.AddType("ResearchOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #region Institute

                dbContext.DBTypeManager.AddType("Institute", "ResearchOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                //#region University

                //dbContext.DBTypeManager.AddType("University", "ResearchOrganization", new Dictionary<AttributeDefinition, string>(), "");

                //#endregion

                #endregion

                #region SportOrganization

                dbContext.DBTypeManager.AddType("SportOrganization", "Organization", new Dictionary<AttributeDefinition, string>(), "");

                #region SportClub

                dbContext.DBTypeManager.AddType("SportClub", "SportOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region SportsFederation

                dbContext.DBTypeManager.AddType("SportsFederation", "SportOrganization", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region StockExchange

                dbContext.DBTypeManager.AddType("StockExchange", "Organization", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region Team

                dbContext.DBTypeManager.AddType("Team", "Organization", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #endregion

                #endregion

                #region Person

                dbContext.DBTypeManager.AddType("Person", "Agent", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasProfession", "LIST<Profession>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasPosition", "LIST<JobPosition>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "isBossOf", "LIST<Person>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasRelatives", "LIST<Person>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasAdress", "LIST<Adress>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasDateOfBirth", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasPartner", "Person");

                #region Man

                dbContext.DBTypeManager.AddType("Man", "Person", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Woman

                dbContext.DBTypeManager.AddType("Woman", "Person", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Brand

                dbContext.DBTypeManager.AddType("Brand", "Object", new Dictionary<AttributeDefinition, string>(), "");

                #region MediaBrand

                dbContext.DBTypeManager.AddType("MediaBrand", "Brand", new Dictionary<AttributeDefinition, string>(), "");

                #region PeriodicalPublication

                dbContext.DBTypeManager.AddType("PeriodicalPublication", "MediaBrand", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("PeriodicalPublication", "hasISSN", DBConstants.DBString);

                #region Magazine

                dbContext.DBTypeManager.AddType("Magazine", "PeriodicalPublication", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Newspaper

                dbContext.DBTypeManager.AddType("Newspaper", "PeriodicalPublication", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region

                #endregion

                #region RatioStation

                dbContext.DBTypeManager.AddType("RatioStation", "MediaBrand", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region TVChannel

                dbContext.DBTypeManager.AddType("TVChannel", "MediaBrand", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #endregion

                #region Currency

                dbContext.DBTypeManager.AddType("Currency", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Currency", "hasUnit", DBConstants.DBString);

                #endregion

                #region Location

                dbContext.DBTypeManager.AddType("Location", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Location", "hasUniversity", "LIST<University>");
                dbContext.DBTypeManager.AddAttributeToType("Location", "latitude", DBConstants.DBDouble);
                dbContext.DBTypeManager.AddAttributeToType("Location", "longitude", DBConstants.DBDouble);
                dbContext.DBTypeManager.AddAttributeToType("Location", "populationCount", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("Location", "geonamesId", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("Location", "height", DBConstants.DBDouble);

                #region Facility

                dbContext.DBTypeManager.AddType("Facility", "Location", new Dictionary<AttributeDefinition, string>(), "");

                #region Building

                dbContext.DBTypeManager.AddType("Building", "Facility", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Monument

                dbContext.DBTypeManager.AddType("Monument", "Facility", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region ReligiousLocation

                dbContext.DBTypeManager.AddType("ReligiousLocation", "Facility", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region TransportFacility

                dbContext.DBTypeManager.AddType("TransportFacility", "Facility", new Dictionary<AttributeDefinition, string>(), "");

                #region Airport

                dbContext.DBTypeManager.AddType("Airport", "TransportFacility", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Bridge

                dbContext.DBTypeManager.AddType("Bridge", "TransportFacility", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region GlobalRegion

                dbContext.DBTypeManager.AddType("GlobalRegion", "Location", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region LandRegion

                dbContext.DBTypeManager.AddType("LandRegion", "Location", new Dictionary<AttributeDefinition, string>(), "");

                #region Continent

                dbContext.DBTypeManager.AddType("Continent", "LandRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Island

                dbContext.DBTypeManager.AddType("Island", "LandRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Mountain

                dbContext.DBTypeManager.AddType("Mountain", "LandRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Valley

                dbContext.DBTypeManager.AddType("Valley", "LandRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region NonGeographicalLocation

                dbContext.DBTypeManager.AddType("NonGeographicalLocation", "Location", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region PoliticalRegion

                dbContext.DBTypeManager.AddType("PoliticalRegion", "Location", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("PoliticalRegion", "hasCapital", "Capital");

                #region Country

                dbContext.DBTypeManager.AddType("Country", "PoliticalRegion", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Country", "hasCurrency", "Currency");
                dbContext.DBTypeManager.AddAttributeToType("Country", "hasGovernment", "GovernmentOrganization");

                #endregion

                #region County

                dbContext.DBTypeManager.AddType("County", "PoliticalRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region MilitaryAreas

                dbContext.DBTypeManager.AddType("MilitaryAreas", "PoliticalRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Province

                dbContext.DBTypeManager.AddType("Province", "PoliticalRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region UrbanDistrict

                dbContext.DBTypeManager.AddType("UrbanDistrict", "PoliticalRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region PopulatedPlace

                dbContext.DBTypeManager.AddType("PopulatedPlace", "Location", new Dictionary<AttributeDefinition, string>(), "");

                #region City

                dbContext.DBTypeManager.AddType("City", "PopulatedPlace", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("City", "hasAirport", "Airport");

                #region Capital

                dbContext.DBTypeManager.AddType("Capital", "City", new Dictionary<AttributeDefinition, string>(), "");

                #region CountryCapital

                dbContext.DBTypeManager.AddType("CountryCapital", "Capital", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region LocalCapital

                dbContext.DBTypeManager.AddType("LocalCapital", "Capital", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #endregion

                #region WaterRegion

                dbContext.DBTypeManager.AddType("WaterRegion", "Location", new Dictionary<AttributeDefinition, string>(), "");

                #region Lake

                dbContext.DBTypeManager.AddType("Lake", "WaterRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Sea

                dbContext.DBTypeManager.AddType("Sea", "WaterRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Stream

                dbContext.DBTypeManager.AddType("Stream", "WaterRegion", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                #region PieceOfArt

                dbContext.DBTypeManager.AddType("PieceOfArt", "Object", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Product

                dbContext.DBTypeManager.AddType("Product", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Product", "producedBy", "LIST<Agent>");

                #region CarModel

                dbContext.DBTypeManager.AddType("CarModel", "Product", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region MediaProduct

                dbContext.DBTypeManager.AddType("MediaProduct", "Product", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region WeaponModelOrSystem

                dbContext.DBTypeManager.AddType("WeaponModelOrSystem", "Product", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Service

                dbContext.DBTypeManager.AddType("Service", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Service", "opteratedBy", "LIST<Agent>");

                #endregion

                #region Statement

                dbContext.DBTypeManager.AddType("Statement", "Object", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Statement", "statedBy", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("Statement", "validFrom", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Statement", "validUntil", DBConstants.DBDateTime);

                #region InformationResource

                dbContext.DBTypeManager.AddType("InformationResource", "Statement", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("InformationResource", "compliantWithSchema", "DataSchema");
                //dbContext.DBTypeManager.AddAttributeToType("InformationResource", "derivedFromSource", "InformationResource");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "hasContributor", "LIST<Agent>");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "hasDate", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "hasSubject", "LIST<Topic>");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "inLanguage", "Language");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "informationResourceCoverage", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "informationResourceIdentification", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "informationResourceRights", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "resourceFormat", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "resourceType", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "title", DBConstants.DBString);

                #region DataSchema

                dbContext.DBTypeManager.AddType("DataSchema", "InformationResource", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Dataset

                dbContext.DBTypeManager.AddType("Dataset", "InformationResource", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Document

                dbContext.DBTypeManager.AddType("Document", "InformationResource", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Document", "documentAbstract", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("Document", "documentSubTitle", DBConstants.DBString);

                #region Contract

                dbContext.DBTypeManager.AddType("Contract", "Document", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Message

                dbContext.DBTypeManager.AddType("Message", "Document", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region PublishedMaterial

                dbContext.DBTypeManager.AddType("PublishedMaterial", "Document", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("PublishedMaterial", "hasPublisher", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("PublishedMaterial", "datePublished", DBConstants.DBDateTime);

                #region Announcement

                dbContext.DBTypeManager.AddType("Announcement", "PublishedMaterial", new Dictionary<AttributeDefinition, string>(), "");


                #endregion

                #region Article

                dbContext.DBTypeManager.AddType("Article", "PublishedMaterial", new Dictionary<AttributeDefinition, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Article", "publishedWithin", "ResourceCollection");

                #endregion

                #region Book

                dbContext.DBTypeManager.AddType("Book", "PublishedMaterial", new Dictionary<AttributeDefinition, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Book", "ISBN", DBConstants.DBString);

                #endregion

                #endregion

                #region Report

                dbContext.DBTypeManager.AddType("Report", "Document", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Patent

                dbContext.DBTypeManager.AddType("Patent", "InformationResource", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region ResourceCollection

                dbContext.DBTypeManager.AddType("ResourceCollection", "InformationResource", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Offer

                dbContext.DBTypeManager.AddType("Offer", "Statement", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #region Order

                dbContext.DBTypeManager.AddType("Order", "Statement", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #region Vehicle

                dbContext.DBTypeManager.AddType("Vehicle", "Object", new Dictionary<AttributeDefinition, string>(), "");

                #endregion

                #endregion

                #endregion

                //Todo: this should be done automatically
                #region LazyAttributes and GraphTypes

                dbContext.DBTypeManager.AddAttributeToType("Role", "roleHolder", "LIST<Object>");
                dbContext.DBTypeManager.AddAttributeToType("OfficialPosition", "officialPositionIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("JobPosition", "withinOrganization", "Organization");
                dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldFrom", "Person");
                dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldto", "Person");
                dbContext.DBTypeManager.AddAttributeToType("JobPosition", "holder", "Person");
                dbContext.DBTypeManager.AddAttributeToType("Entity", "locatedIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("Entity", "partOf", "Entity");
                dbContext.DBTypeManager.AddAttributeToType("Object", "isOwnedBy", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("Account", "accountProvider", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "establishedIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "registeredIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("GovernmentOrganization", "ofCountry", "Country");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasTags", "LIST<Tag>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasLeisure", "LIST<Leisure>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasFulltxtLeisure", "LIST<FulltxtLeisure>");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "likeSex", "SexualOrientation");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogUser", "hasPerson", "Person");
                dbContext.DBTypeManager.AddAttributeToType("Account", "accountProvider", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("Ministry", "hasMinister", "LIST<Person>");
                dbContext.DBTypeManager.AddAttributeToType("PoliticalRegion", "hasCapital", "Capital");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "derivedFromSource", "InformationResource");
                dbContext.DBTypeManager.AddAttributeToType("InformationResource", "compliantWithSchema", "DataSchema");
                dbContext.DBTypeManager.AddAttributeToType("Article", "publishedWithin", "ResourceCollection");

                #endregion

                _Transaction.Commit();

            }

        }

        #endregion

    }

}
