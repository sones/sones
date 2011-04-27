
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
        //encode the query
        var encodedQuery = $.URLEncode(query);

        //build the target URI
        var target = goosh.config.webservice_protocol + "://"
               + goosh.config.webservice_host
               + ((goosh.config.webservice_port != undefined) ? (":" + goosh.config.webservice_port) : "")
               + goosh.config.webservice_path + "/"
               + goosh.config.mode + "?"
               + encodedQuery;

        //do some ajax
        var RESTResponse = $.ajax({
            url: target,
            cache: false,
            async: false,
            timeout: 0,
            error: function (xhr, ajaxOptions, thrownError) {
                return ("AJAX Error " + xhr.status + "\n" + data.responseText + "\n" + thrownError);
            },
            beforeSend: function (xhr) {
                if (goosh.config.webservice_default_format == "xml")
                    xhr.setRequestHeader('Accept', 'application/xml');
                else if (goosh.config.webservice_default_format == "gexf")
                    xhr.setRequestHeader('Accept', 'application/gexf');
                else if (goosh.config.webservice_default_format == "text")
                    xhr.setRequestHeader('Accept', 'text/plain');
                else
                    xhr.setRequestHeader('Accept', 'application/json');
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
                return '<pre class=\"AttrTagValue\">' + result + '</pre>';

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
