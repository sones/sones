/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="sones GraphDB – OM CLI command" />
 * <copyright file="DBCLI_OM.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This command enables the user to load and store 
 * an ontology in OWL.</summary>
 */



#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.CLI;
using sones.Lib.Frameworks.CLIrony.Compiler;

using sones.GraphDB;
using sones.GraphFS.Session;
using sones.GraphDB.TypeManagement;

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

            OM.PandoraOptions.Add(PandoraOption.IsCommandRoot);

            OM_Action.Rule = StoreSymbol + OM_Store_options
                                | LoadSymbol + OM_Load_options;
            OM_Action.PandoraOptions.Add(PandoraOption.IsOption);

            OM_Load_options.Rule = OM_ProtonString
                                    | stringLiteralExternalEntry;
            OM_Load_options.PandoraOptions.Add(PandoraOption.IsOption);

            OM_Store_options.Rule = stringLiteralExternalEntry;
            OM_Store_options.PandoraOptions.Add(PandoraOption.IsOption);

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

        public override void Execute(ref object myIGraphFS2Session, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IPandoraDBSession = myIPandoraDBSession as IGraphDBSession;

            if (_IGraphFS2Session == null || _IPandoraDBSession == null)
            {
                WriteLine("No OM database instance started...");
                return;
            }

            switch (myOptions.ElementAt(1).Value[0].Option)
            {

                case "load":
                    switch (myOptions.ElementAt(2).Value[0].Option)
                    {
                        case "proton":
                            LoadTheProtonOntology((GraphDBSession)myIPandoraDBSession, myIGraphFS2Session);
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

        }

        private void LoadTheProtonOntology(GraphDBSession DB, Object myIPandoraFS)
        {
            var dbContext = DB.GetDBContext();
            using (var _Transaction = DB.BeginTransaction())
            {

                #region Entity

                dbContext.DBTypeManager.AddType("Entity", DBConstants.DBBaseObject, new Dictionary<TypeAttribute, String>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Entity", "label", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("Entity", "comment", DBConstants.DBString);
                //dbContext.DBTypeManager.AddAttributeToType("Entity", "locatedIn", "Location");
                //dbContext.DBTypeManager.AddAttributeToType("Entity", "partOf", "Entity");


                #region Abstract

                dbContext.DBTypeManager.AddType("Abstract", "Entity", new Dictionary<TypeAttribute, string>(), "");

                #region BusinessAbstraction

                dbContext.DBTypeManager.AddType("BusinessAbstraction", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region IndustrySector

                dbContext.DBTypeManager.AddType("IndustrySector", "BusinessAbstraction", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("IndustrySector", "subSectorOf", "IndustrySector");
                dbContext.DBTypeManager.AddAttributeToType("IndustrySector", "hasCode", DBConstants.DBString);

                #endregion

                #region Market

                dbContext.DBTypeManager.AddType("Market", "BusinessAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region UserAbstraction

                dbContext.DBTypeManager.AddType("UserAbstraction", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region KissNoFrogUser

                dbContext.DBTypeManager.AddType("KissNoFrogUser", "UserAbstraction", new Dictionary<TypeAttribute, string>(), "");
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

                dbContext.DBTypeManager.AddType("KissNoFrogVisitor", "KissNoFrogUser", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("KissNoFrogVisitor", "whenVisited", DBConstants.DBDateTime);

                #endregion

                #endregion

                #endregion

                #region ContactInformation

                dbContext.DBTypeManager.AddType("ContactInformation", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region Adress

                dbContext.DBTypeManager.AddType("Adress", "ContactInformation", new Dictionary<TypeAttribute, string>(), "");

                #region PostalAdress

                dbContext.DBTypeManager.AddType("PostalAdress", "Adress", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region InternetAdress

                dbContext.DBTypeManager.AddType("InternetAdress", "ContactInformation", new Dictionary<TypeAttribute, string>(), "");

                #region EMail

                dbContext.DBTypeManager.AddType("EMail", "InternetAdress", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region IPAdress

                dbContext.DBTypeManager.AddType("IPAdress", "InternetAdress", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region InternetDomain

                dbContext.DBTypeManager.AddType("InternetDomain", "InternetAdress", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region WebPage

                dbContext.DBTypeManager.AddType("WebPage", "InternetAdress", new Dictionary<TypeAttribute, string>(), "");

                #region HomePage

                dbContext.DBTypeManager.AddType("HomePage", "WebPage", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region PhoneNumber

                dbContext.DBTypeManager.AddType("PhoneNumber", "ContactInformation", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region GeneralTerm

                dbContext.DBTypeManager.AddType("GeneralTerm", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region Tag

                dbContext.DBTypeManager.AddType("Tag", "GeneralTerm", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Leisure

                dbContext.DBTypeManager.AddType("Leisure", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region FulltxtLeisure

                dbContext.DBTypeManager.AddType("FulltxtLeisure", "Leisure", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Language

                dbContext.DBTypeManager.AddType("Language", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region SocialAbstraction

                dbContext.DBTypeManager.AddType("SocialAbstraction", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region Art

                dbContext.DBTypeManager.AddType("Art", "SocialAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Money

                dbContext.DBTypeManager.AddType("Money", "SocialAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Profession

                dbContext.DBTypeManager.AddType("Profession", "SocialAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region ResearchArea

                dbContext.DBTypeManager.AddType("ResearchArea", "SocialAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Sport

                dbContext.DBTypeManager.AddType("Sport", "SocialAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region TemporalAbstraction

                dbContext.DBTypeManager.AddType("TemporalAbstraction", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region CalendarMonth

                dbContext.DBTypeManager.AddType("CalendarMonth", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region DayOfMonth

                dbContext.DBTypeManager.AddType("DayOfMonth", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region DayOfWeek

                dbContext.DBTypeManager.AddType("DayOfWeek", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region DayTime

                dbContext.DBTypeManager.AddType("DayTime", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Festival

                dbContext.DBTypeManager.AddType("Festival", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Season

                dbContext.DBTypeManager.AddType("Season", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region TimeZone

                dbContext.DBTypeManager.AddType("TimeZone", "TemporalAbstraction", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Topic

                dbContext.DBTypeManager.AddType("Topic", "Abstract", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Topic", "isSubTopicOf", "Topic");

                #endregion

                #region NaturalPhenomenon

                dbContext.DBTypeManager.AddType("NaturalPhenomenon", "Abstract", new Dictionary<TypeAttribute, string>(), "");

                #region SexualOrientation

                dbContext.DBTypeManager.AddType("SexualOrientation", "NaturalPhenomenon", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Happening

                dbContext.DBTypeManager.AddType("Happening", "Entity", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Happening", "startTime", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Happening", "endTime", DBConstants.DBDateTime);

                #region Event

                dbContext.DBTypeManager.AddType("Event", "Happening", new Dictionary<TypeAttribute, string>(), "");

                #region Accident

                dbContext.DBTypeManager.AddType("Accident", "Event", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region ArtPerformace

                dbContext.DBTypeManager.AddType("ArtPerformace", "Event", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Meeting

                dbContext.DBTypeManager.AddType("Meeting", "Event", new Dictionary<TypeAttribute, string>(), "");

                #region KNFMeeting

                dbContext.DBTypeManager.AddType("KNFMeeting", "Meeting", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "ageLowerBound", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "ageUpperBound", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "isVisible", DBConstants.DBBoolean);
                dbContext.DBTypeManager.AddAttributeToType("KNFMeeting", "hasParticipants", "LIST<KissNoFrogUser>");

                #endregion

                #region BoardMeeting

                dbContext.DBTypeManager.AddType("BoardMeeting", "Meeting", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region OfficialPoliticalMeeting

                dbContext.DBTypeManager.AddType("OfficialPoliticalMeeting", "Meeting", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region MilitaryConflict

                dbContext.DBTypeManager.AddType("MilitaryConflict", "Event", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Project

                dbContext.DBTypeManager.AddType("Project", "Event", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region SportEvent

                dbContext.DBTypeManager.AddType("SportEvent", "Event", new Dictionary<TypeAttribute, string>(), "");

                #region OlympicGames

                dbContext.DBTypeManager.AddType("OlympicGames", "SportEvent", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region SportGame

                dbContext.DBTypeManager.AddType("SportGame", "SportEvent", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Tournament

                dbContext.DBTypeManager.AddType("Tournament", "SportEvent", new Dictionary<TypeAttribute, string>(), "");

                #endregion


                #endregion

                #endregion

                #region Situation

                dbContext.DBTypeManager.AddType("Situation", "Happening", new Dictionary<TypeAttribute, string>(), "");

                #region JobPosition

                dbContext.DBTypeManager.AddType("JobPosition", "Situation", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "withinOrganization", "Organization");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldFrom", "Person");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "heldto", "Person");
                //dbContext.DBTypeManager.AddAttributeToType("JobPosition", "holder", "Person");

                #region BoardMember

                dbContext.DBTypeManager.AddType("BoardMember", "JobPosition", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Employee

                dbContext.DBTypeManager.AddType("Employee", "JobPosition", new Dictionary<TypeAttribute, string>(), "");

                #region Manager

                dbContext.DBTypeManager.AddType("Manager", "Employee", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Leader

                dbContext.DBTypeManager.AddType("Leader", "JobPosition", new Dictionary<TypeAttribute, string>(), "");

                #region Chairman

                dbContext.DBTypeManager.AddType("Chairman", "Leader", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Executive

                dbContext.DBTypeManager.AddType("Executive", "Leader", new Dictionary<TypeAttribute, string>(), "");

                #region CEO

                dbContext.DBTypeManager.AddType("CEO", "Executive", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Minister

                dbContext.DBTypeManager.AddType("Minister", "Executive", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Premier

                dbContext.DBTypeManager.AddType("Premier", "Executive", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region President

                dbContext.DBTypeManager.AddType("President", "Leader", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region MemberOfParliament

                dbContext.DBTypeManager.AddType("MemberOfParliament", "JobPosition", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region OfficialPosition

                dbContext.DBTypeManager.AddType("OfficialPosition", "JobPosition", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("OfficialPosition", "officialPositionIn", "Location");

                #endregion

                #endregion

                #region Role

                dbContext.DBTypeManager.AddType("Role", "Situation", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Role", "roleIn", "LIST<Happening>");
                //dbContext.DBTypeManager.AddAttributeToType("Role", "roleHolder", "LIST<Object>"); //loaded 

                #endregion

                #endregion

                #region TimeInterval

                dbContext.DBTypeManager.AddType("TimeInterval", "Happening", new Dictionary<TypeAttribute, string>(), "");

                #region CalendarYear

                dbContext.DBTypeManager.AddType("CalendarYear", "TimeInterval", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Date

                dbContext.DBTypeManager.AddType("Date", "TimeInterval", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Month

                dbContext.DBTypeManager.AddType("Month", "TimeInterval", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Quarter

                dbContext.DBTypeManager.AddType("Quarter", "TimeInterval", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Week

                dbContext.DBTypeManager.AddType("Week", "TimeInterval", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Object

                dbContext.DBTypeManager.AddType("Object", "Entity", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Object", "hasContactInfo", "ContactInformation");
                //dbContext.DBTypeManager.AddAttributeToType("Object", "isOwnedBy", "Agent");

                #region Account

                dbContext.DBTypeManager.AddType("Account", "Object", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Account", "accountProvider", "Agent");

                #endregion

                #region Agent

                dbContext.DBTypeManager.AddType("Agent", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Agent", "involvedIn", "LIST<Happening>");
                dbContext.DBTypeManager.AddAttributeToType("Agent", "isLegalEntity", DBConstants.DBBoolean);
                dbContext.DBTypeManager.AddAttributeToType("Agent", "partiallyControls", "LIST<Object>");

                #region Group

                dbContext.DBTypeManager.AddType("Group", "Agent", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Group", "hasMember", "LIST<Agent>");

                #region Organization

                dbContext.DBTypeManager.AddType("Organization", "Group", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "childOrganizationOf", "Organization");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "doingBusinessAs", DBConstants.DBString);
                //dbContext.DBTypeManager.AddAttributeToType("Organization", "establishedIn", "Location");
                dbContext.DBTypeManager.AddAttributeToType("Organization", "establishmentDate", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Organization", "numberOfEmployees", DBConstants.DBInteger);
                //dbContext.DBTypeManager.AddAttributeToType("Organization", "registeredIn", "Location");

                #region Charity

                dbContext.DBTypeManager.AddType("Charity", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region CommercialOrganization

                dbContext.DBTypeManager.AddType("CommercialOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("CommercialOrganization", "activeInSector", "IndustrySector");
                dbContext.DBTypeManager.AddAttributeToType("CommercialOrganization", "hasShareholder", "LIST<Agent>");

                #region Company

                dbContext.DBTypeManager.AddType("Company", "CommercialOrganization", new Dictionary<TypeAttribute, string>(), "");

                #region Airline

                dbContext.DBTypeManager.AddType("Airline", "Company", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region Bank

                dbContext.DBTypeManager.AddType("Bank", "Company", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region Telecom

                dbContext.DBTypeManager.AddType("Telecom", "Company", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #endregion

                #endregion

                #region Division

                dbContext.DBTypeManager.AddType("Division", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region EducationalOrganization

                dbContext.DBTypeManager.AddType("EducationalOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #region School

                dbContext.DBTypeManager.AddType("School", "EducationalOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region University

                dbContext.DBTypeManager.AddType("University", "EducationalOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region GovernmentOrganization

                dbContext.DBTypeManager.AddType("GovernmentOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("GovernmentOrganization", "ofCountry", "Country");

                #region Government

                dbContext.DBTypeManager.AddType("Government", "GovernmentOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Ministry

                dbContext.DBTypeManager.AddType("Ministry", "GovernmentOrganization", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Ministry", "hasMinister", "LIST<Person>");

                #endregion

                #endregion

                #region InternationalOrganization

                dbContext.DBTypeManager.AddType("InternationalOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region PoliticalEntity

                dbContext.DBTypeManager.AddType("PoliticalEntity", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #region Parliament

                dbContext.DBTypeManager.AddType("Parliament", "PoliticalEntity", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region PoliticalParty

                dbContext.DBTypeManager.AddType("PoliticalParty", "PoliticalEntity", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region ReligiousOrganization

                dbContext.DBTypeManager.AddType("ReligiousOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region ResearchOrganization

                dbContext.DBTypeManager.AddType("ResearchOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #region Institute

                dbContext.DBTypeManager.AddType("Institute", "ResearchOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                //#region University

                //dbContext.DBTypeManager.AddType("University", "ResearchOrganization", new Dictionary<TypeAttribute, string>(), "");

                //#endregion

                #endregion

                #region SportOrganization

                dbContext.DBTypeManager.AddType("SportOrganization", "Organization", new Dictionary<TypeAttribute, string>(), "");

                #region SportClub

                dbContext.DBTypeManager.AddType("SportClub", "SportOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region SportsFederation

                dbContext.DBTypeManager.AddType("SportsFederation", "SportOrganization", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region StockExchange

                dbContext.DBTypeManager.AddType("StockExchange", "Organization", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region Team

                dbContext.DBTypeManager.AddType("Team", "Organization", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #endregion

                #endregion

                #region Person

                dbContext.DBTypeManager.AddType("Person", "Agent", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasProfession", "LIST<Profession>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasPosition", "LIST<JobPosition>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "isBossOf", "LIST<Person>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasRelatives", "LIST<Person>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasAdress", "LIST<Adress>");
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasDateOfBirth", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Person", "hasPartner", "Person");

                #region Man

                dbContext.DBTypeManager.AddType("Man", "Person", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Woman

                dbContext.DBTypeManager.AddType("Woman", "Person", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region Brand

                dbContext.DBTypeManager.AddType("Brand", "Object", new Dictionary<TypeAttribute, string>(), "");

                #region MediaBrand

                dbContext.DBTypeManager.AddType("MediaBrand", "Brand", new Dictionary<TypeAttribute, string>(), "");

                #region PeriodicalPublication

                dbContext.DBTypeManager.AddType("PeriodicalPublication", "MediaBrand", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("PeriodicalPublication", "hasISSN", DBConstants.DBString);

                #region Magazine

                dbContext.DBTypeManager.AddType("Magazine", "PeriodicalPublication", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Newspaper

                dbContext.DBTypeManager.AddType("Newspaper", "PeriodicalPublication", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region

                #endregion

                #region RatioStation

                dbContext.DBTypeManager.AddType("RatioStation", "MediaBrand", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region TVChannel

                dbContext.DBTypeManager.AddType("TVChannel", "MediaBrand", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #endregion

                #region Currency

                dbContext.DBTypeManager.AddType("Currency", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Currency", "hasUnit", DBConstants.DBString);

                #endregion

                #region Location

                dbContext.DBTypeManager.AddType("Location", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Location", "hasUniversity", "LIST<University>");
                dbContext.DBTypeManager.AddAttributeToType("Location", "latitude", DBConstants.DBDouble);
                dbContext.DBTypeManager.AddAttributeToType("Location", "longitude", DBConstants.DBDouble);
                dbContext.DBTypeManager.AddAttributeToType("Location", "populationCount", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("Location", "geonamesId", DBConstants.DBInteger);
                dbContext.DBTypeManager.AddAttributeToType("Location", "height", DBConstants.DBDouble);

                #region Facility

                dbContext.DBTypeManager.AddType("Facility", "Location", new Dictionary<TypeAttribute, string>(), "");

                #region Building

                dbContext.DBTypeManager.AddType("Building", "Facility", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Monument

                dbContext.DBTypeManager.AddType("Monument", "Facility", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region ReligiousLocation

                dbContext.DBTypeManager.AddType("ReligiousLocation", "Facility", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region TransportFacility

                dbContext.DBTypeManager.AddType("TransportFacility", "Facility", new Dictionary<TypeAttribute, string>(), "");

                #region Airport

                dbContext.DBTypeManager.AddType("Airport", "TransportFacility", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Bridge

                dbContext.DBTypeManager.AddType("Bridge", "TransportFacility", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region GlobalRegion

                dbContext.DBTypeManager.AddType("GlobalRegion", "Location", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region LandRegion

                dbContext.DBTypeManager.AddType("LandRegion", "Location", new Dictionary<TypeAttribute, string>(), "");

                #region Continent

                dbContext.DBTypeManager.AddType("Continent", "LandRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Island

                dbContext.DBTypeManager.AddType("Island", "LandRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Mountain

                dbContext.DBTypeManager.AddType("Mountain", "LandRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Valley

                dbContext.DBTypeManager.AddType("Valley", "LandRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region NonGeographicalLocation

                dbContext.DBTypeManager.AddType("NonGeographicalLocation", "Location", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region PoliticalRegion

                dbContext.DBTypeManager.AddType("PoliticalRegion", "Location", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("PoliticalRegion", "hasCapital", "Capital");

                #region Country

                dbContext.DBTypeManager.AddType("Country", "PoliticalRegion", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Country", "hasCurrency", "Currency");
                dbContext.DBTypeManager.AddAttributeToType("Country", "hasGovernment", "GovernmentOrganization");

                #endregion

                #region County

                dbContext.DBTypeManager.AddType("County", "PoliticalRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region MilitaryAreas

                dbContext.DBTypeManager.AddType("MilitaryAreas", "PoliticalRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Province

                dbContext.DBTypeManager.AddType("Province", "PoliticalRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region UrbanDistrict

                dbContext.DBTypeManager.AddType("UrbanDistrict", "PoliticalRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region PopulatedPlace

                dbContext.DBTypeManager.AddType("PopulatedPlace", "Location", new Dictionary<TypeAttribute, string>(), "");

                #region City

                dbContext.DBTypeManager.AddType("City", "PopulatedPlace", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("City", "hasAirport", "Airport");

                #region Capital

                dbContext.DBTypeManager.AddType("Capital", "City", new Dictionary<TypeAttribute, string>(), "");

                #region CountryCapital

                dbContext.DBTypeManager.AddType("CountryCapital", "Capital", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region LocalCapital

                dbContext.DBTypeManager.AddType("LocalCapital", "Capital", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #endregion

                #region WaterRegion

                dbContext.DBTypeManager.AddType("WaterRegion", "Location", new Dictionary<TypeAttribute, string>(), "");

                #region Lake

                dbContext.DBTypeManager.AddType("Lake", "WaterRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Sea

                dbContext.DBTypeManager.AddType("Sea", "WaterRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Stream

                dbContext.DBTypeManager.AddType("Stream", "WaterRegion", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                #region PieceOfArt

                dbContext.DBTypeManager.AddType("PieceOfArt", "Object", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Product

                dbContext.DBTypeManager.AddType("Product", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Product", "producedBy", "LIST<Agent>");

                #region CarModel

                dbContext.DBTypeManager.AddType("CarModel", "Product", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region MediaProduct

                dbContext.DBTypeManager.AddType("MediaProduct", "Product", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region WeaponModelOrSystem

                dbContext.DBTypeManager.AddType("WeaponModelOrSystem", "Product", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Service

                dbContext.DBTypeManager.AddType("Service", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Service", "opteratedBy", "LIST<Agent>");

                #endregion

                #region Statement

                dbContext.DBTypeManager.AddType("Statement", "Object", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Statement", "statedBy", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("Statement", "validFrom", DBConstants.DBDateTime);
                dbContext.DBTypeManager.AddAttributeToType("Statement", "validUntil", DBConstants.DBDateTime);

                #region InformationResource

                dbContext.DBTypeManager.AddType("InformationResource", "Statement", new Dictionary<TypeAttribute, string>(), "");
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

                dbContext.DBTypeManager.AddType("DataSchema", "InformationResource", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Dataset

                dbContext.DBTypeManager.AddType("Dataset", "InformationResource", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Document

                dbContext.DBTypeManager.AddType("Document", "InformationResource", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Document", "documentAbstract", DBConstants.DBString);
                dbContext.DBTypeManager.AddAttributeToType("Document", "documentSubTitle", DBConstants.DBString);

                #region Contract

                dbContext.DBTypeManager.AddType("Contract", "Document", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Message

                dbContext.DBTypeManager.AddType("Message", "Document", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region PublishedMaterial

                dbContext.DBTypeManager.AddType("PublishedMaterial", "Document", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("PublishedMaterial", "hasPublisher", "Agent");
                dbContext.DBTypeManager.AddAttributeToType("PublishedMaterial", "datePublished", DBConstants.DBDateTime);

                #region Announcement

                dbContext.DBTypeManager.AddType("Announcement", "PublishedMaterial", new Dictionary<TypeAttribute, string>(), "");


                #endregion

                #region Article

                dbContext.DBTypeManager.AddType("Article", "PublishedMaterial", new Dictionary<TypeAttribute, string>(), "");
                //dbContext.DBTypeManager.AddAttributeToType("Article", "publishedWithin", "ResourceCollection");

                #endregion

                #region Book

                dbContext.DBTypeManager.AddType("Book", "PublishedMaterial", new Dictionary<TypeAttribute, string>(), "");
                dbContext.DBTypeManager.AddAttributeToType("Book", "ISBN", DBConstants.DBString);

                #endregion

                #endregion

                #region Report

                dbContext.DBTypeManager.AddType("Report", "Document", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Patent

                dbContext.DBTypeManager.AddType("Patent", "InformationResource", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region ResourceCollection

                dbContext.DBTypeManager.AddType("ResourceCollection", "InformationResource", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Offer

                dbContext.DBTypeManager.AddType("Offer", "Statement", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #region Order

                dbContext.DBTypeManager.AddType("Order", "Statement", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #region Vehicle

                dbContext.DBTypeManager.AddType("Vehicle", "Object", new Dictionary<TypeAttribute, string>(), "");

                #endregion

                #endregion

                #endregion

                //Todo: this should be done automatically
                #region LazyAttributes and PandoraTypes

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
