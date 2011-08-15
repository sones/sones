
    //xml output
    function printXMLResult(queryResult) {

        if (queryResult != undefined) {

            var out = "";        
            out += "<pre>" + escapeArrowBrackets((new XMLSerializer()).serializeToString(queryResult)) + "</pre>";//.replace(/(\r\n|[\r\n])/g, "<br />");
            
            goosh.gui.out(out);

        }
    }

   
   

    //query handling
    function doQuery(args) {
        if (args.length > 0) {
            //build the query string
            var query = "";
            for (var i = 0; i < args.length; i++) {
                query += args[i] + " ";
            }
            //send the trimmed query
            return sendQuery(jQuery.trim(query));
        } else {
            return "";
        }
    }

    function escapeArrowBrackets(text) {
        text = text.replace(/</g, "&lt;");
        text = text.replace(/>/g, "&gt;");
        return text;
    }


    function sendQuery(query) {

        //replace apostrophe
        query = query.replace(/´/g, "'");
        query = query.replace(/\"/g, "'");
    
        //build the target URI
        var target = goosh.config.webservice_protocol + "://"
               + goosh.config.webservice_host
               + ((goosh.config.webservice_port != undefined) ? (":" + goosh.config.webservice_port) : "")
               + goosh.config.webservice_path + "/"
               + goosh.config.mode;
               

        //do some ajax
        var RESTResponse = $.ajax({
            type: "POST",
            url: target,
            cache: false,
            async: false,
            timeout: 0,
            data: query,
            error: function (xhr, ajaxOptions, thrownError) {
                return ("AJAX Error " + xhr.status + "\n" + data.responseText + "\n" + thrownError);
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Accept', goosh.config.webservice_default_format.type);
            }
        });

        if (RESTResponse == null)
            return "<span class=\"AttrTagValue\">Error: Empty result set!</span>";

        return handleRESTResult(RESTResponse);

    }

    function handleRESTResult(RESTResponse) {
        //strip the encoded result out of the result xml
        //the pattern strips the text between the string tag
        //var pattern = /(<string\b[^>]*>(.*?)<\/string>)/;
        //var result = pattern.exec(resultString);

        //result = $.base64Decode(resultString);
        result = RESTResponse.responseText;
        ContentType = RESTResponse.getResponseHeader('Content-Type');

        if (result != undefined && result != "") {

            // ServerSideException
            if (result.indexOf("ServerSideException") > -1)
                return "<span class=\"AttrTagValue\">Server Side Exception!</span>";

            // xml
            if (ContentType != null && ContentType.indexOf("application/xml") > -1) {

                // Parse XML from String       
                if (window.DOMParser) {
                    parser = new DOMParser();
                    xmlDoc = parser.parseFromString(result, "text/xml");
                }

                // Internet Explorer
                else {
                    xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                    var pi = xmlDoc.createProcessingInstruction("xml", " version='1.0' encoding='UTF-8'");
                    var XmlPattern = /(<?xml\sversion*)/;
                    var matchResult = XmlPattern.exec(result);

                    if (matchResult != undefined) {
                        var index = result.indexOf(">", 0);
                        if (index > 0)
                            result = result.slice(index + 1, result.length);
                    }

                    xmlDoc.appendChild(pi);
                    xmlDoc.async = "false";
                    xmlDoc.loadXML(result);
                }

                return xmlDoc;

            }

            // json
            else if (ContentType.indexOf("application/json") > -1)
                return '<pre class=\"AttrTagValue\">' + result + '</pre>';

            // text
            else if (ContentType.indexOf("text/plain") > -1)
                return '<pre>' + result + '</pre>';

            // html
            else if (ContentType.indexOf("text/html") > -1)
                return '<pre class=\"AttrTagValue\">' + result + '</pre>';

            // barchart
            else if (ContentType.indexOf("application/x-sones-barchart") > -1) {
                $('body').append('<script type=\"text/javascript\" src=\"resources/d3/d3.js\"/>');
                $('body').append('<script type=\"text/javascript\">'+ result + '</script/>');
                return '';
            }

            // error
            else
                return "<span class=\"AttrTagValue\">Error: Unknown content-type '" + ContentType + "'!</span>";

        }

        else
            return "<span class=\"AttrTagValue\">Communication error or error parsing queryresult from REST!</span>";

    }


    function UpdateDDate() {

        //build the target URI
        var target = goosh.config.webservice_protocol + "://"
               + goosh.config.webservice_host
               + ((goosh.config.webservice_port != undefined) ? (":" + goosh.config.webservice_port) : "")
               + "/"
               + goosh.config.webservice_path + "ddate";

        //do some ajax
        var html = $.ajax({
            url: target,
            cache: false,
            async: false,
            error: function () {
                return 1;
            }
        });

        if (html == null) {
            return 2;
        } else {
            goosh.gui.ddate.innerHTML = html.responseText;
            return 0;
        }

    };

 //extend Webshell
    $(document).ready(function () {
        //history module
        goosh.module.history = function () {

            this.name = "history";
            this.aliases = new Array("history");
            this.help = "show last commands";

            this.call = function (args) {
                var out = "";
                if (goosh.keyboard.hist.length > 0) {
                    out = "<ol class=\"historylist\">";
                    for (i = 0; i < goosh.keyboard.hist.length; i++) {
                        out += "<li>" + goosh.keyboard.hist[i] + "</li>";
                    }
                    out += "</ol>";
                }

                goosh.gui.outln(out);
            };
        };

        goosh.modules.register("history");
        //eo history module


        //the GUI is waiting for an AJAX-Response
        goosh.gui.waiting = false;
        goosh.gui.setWaiting = function (waitingFlag) {
            goosh.gui.waiting = (waitingFlag == true) ? true : false;
            if (goosh.gui.waiting) {
                $("<img src=\"/resources/WebShell/waitingimg.gif\" alt=\"processing...\" class=\"waitingimg\" />").appendTo("#prompt");
            } else {
                $(".waitingimg").remove();
            }
        };


        //update goosh-configuration
        goosh.config.user = "GraphDB";
        goosh.config.host = jQuery.url.attr("host");
        goosh.config.mode = "gql";
        goosh.config.webservice_protocol = jQuery.url.attr("protocol");
        goosh.config.webservice_host = jQuery.url.attr("host");
        goosh.config.webservice_path = jQuery.url.attr("directory").substring(0, jQuery.url.attr("directory").lastIndexOf('/'));
        goosh.config.webservice_port = jQuery.url.attr("port");

        //sones.licence
        goosh.module.license = function () {

            this.name = "license";
            this.aliases = new Array("license", "l");

            this.help = "displays license information";

            this.call = function (args) {

                var out = "";
                out += "<pre>";
                out += "Copyright (c) 2007-2011, sones GmbH - www.sones.com\n";
                out += "All rights reserved.\n";
                out += "\n";
                out += "New BSD License\n";
                out += "\n";
                out += "Redistribution and use in source and binary forms, with or without\n";
                out += "modification, are permitted provided that the following conditions are met:\n";
                out += "    * Redistributions of source code must retain the above copyright\n";
                out += "      notice, this list of conditions and the following disclaimer.\n";
                out += "    * Redistributions in binary form must reproduce the above copyright\n";
                out += "      notice, this list of conditions and the following disclaimer in the\n";
                out += "      documentation and/or other materials provided with the distribution.\n";
                out += "    * Neither the name of the sones GmbH nor the\n";
                out += "      names of its contributors may be used to endorse or promote products\n";
                out += "      derived from this software without specific prior written permission.\n";
                out += "\n";
                out += "THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND\n";
                out += "ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED\n";
                out += "WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE\n";
                out += "DISCLAIMED. IN NO EVENT SHALL sones GmbH BE LIABLE FOR ANY\n";
                out += "DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES\n";
                out += "(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;\n";
                out += "LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND\n";
                out += "ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT\n";
                out += "(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS\n";
                out += "SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.\n";
                out += "\n";
                out += "\n";
                out += "jQuery JavaScript Library - Copyright (c) 2009 John Resig\n";
                out += "Dual licensed under the MIT and GPL licenses.\n";
                out += "http://docs.jquery.com/License\n";
                out += "\n";
                out += "goosh is written by Stefan Grothkopp <grothkopp@gmail.com>\n";
                out += "goosh is open source under the Artistic License/GPL.\n";
                out += "http://www.goosh.org\n";
                out += "</pre>";
                goosh.gui.outln(out);
            }
        }

        goosh.modules.register("license");
        //eo sones.licence


        //GQL handler
        goosh.module.gql = function () {
            this.name = "gql";
            this.aliases = new Array("gql", "g");
            this.help = "switch to gql mode";

            this.call = function (args) {
                if (goosh.config.mode != "gql") {
                    goosh.config.mode = "gql";
                    goosh.gui.updateprompt();
                } else {
                    //result is XML
                    var result = doQuery(args);
                    if (result != undefined) {
                        if (goosh.config.webservice_default_format.type.indexOf('application/xml') > -1) {
                            printXMLResult(result.firstChild);
                        } else if (goosh.config.webservice_default_format.type.indexOf('application/gexf') > -1) {
                            printXMLResult(result.firstChild);
                        }
                        else if (goosh.config.webservice_default_format.type.indexOf('application/json') > -1) { //json
                            /*
                            * json is currently displayed as one line string
                            * String can be parsed to JSON Object via eval()                    
                            */
                            //goosh.gui.out(printJSONResult(eval('(' + result + ')')));                    
                            goosh.gui.out(result);
                        } else { //text
                            goosh.gui.out(result);
                        }
                    } else { //result is undefined
                        goosh.gui.error("Internal");
                    }
                }
            }
        }
        goosh.modules.register("gql");
        //eo GQL handler
    });
