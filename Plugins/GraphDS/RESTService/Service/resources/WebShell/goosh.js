
//<!--
/*
    Goosh.org (c) 2008 - Stefan Grothkopp

    This script is a google-interface that behaves similar to a unix-shell.

    goosh is written by Stefan Grothkopp (grothkopp (at) gmail (dot) com)
    it is NOT an official google product!

    If you want to extend goosh.org, please take a look at the load command.
    You can see an example module at http://goosh.org/ext/spon.js

    Uncompressed source can be found at:
    http://code.google.com/p/goosh/
    Instructions for svn access are at:
    http://code.google.com/p/goosh/source/checkout

    If you have problems/bug reports/etc please write me an email.

*/

InitGoosh = function (goosh) {

    goosh.lib = new Object();

    //namespace.js
    // thanks to Michael Schwarz for this function
    goosh.lib.namespace = function (A) { var B = A.split("."); var C = window; for (var D = 0; D < B.length; D++) { if (typeof C[B[D]] == "undefined") C[B[D]] = new Object(); C = C[B[D]]; } };
    //eo namespace.js

    //in_array.js
    goosh.lib.in_array = function (A, B) { var C; for (C = 0; C < A.length; C++) { if (A[C] == B) { return true; } } return false; };
    //eo in_array.js

    //config.js
    goosh.lib.namespace("goosh.config");
    goosh.config.apikey = "notnecessary";
    goosh.config.user = "GraphDB";
    goosh.config.host = jQuery.url.attr("host");
    goosh.config.mode = "gql";
    goosh.config.pend = ">&nbsp;";
    goosh.config.webservice_protocol = jQuery.url.attr("protocol");
    goosh.config.webservice_host = jQuery.url.attr("host");
    goosh.config.webservice_path = jQuery.url.attr("directory").substring(0, jQuery.url.attr("directory").lastIndexOf('/'));
    goosh.config.webservice_port = jQuery.url.attr("port");
    goosh.config.webservice_formats = new Array();
    goosh.config.webservice_default_format = { name: 'not set', type: 'not set' };
    goosh.config.numres = 4;
    goosh.config.timeout = 4;
    goosh.config.start = 0;
    goosh.config.moreobj;
    goosh.config.lang = "en";
    goosh.config.urls = new Array();
    goosh.config.cmdlines = new Array();
    goosh.config.cmdqueue = new Array();
    //eo config.js

    //get.js
    goosh.lib.namespace("goosh.lib");
    goosh.lib.get = function (A) {
        A = A.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var B = "[\\?&]" + A + "=([^&#]*)";
        var C = new RegExp(B);
        var D = C.exec(window.location.href);
        if (D == null) return "";
        else return decodeURIComponent(D[1]).replace(/\+/g, " ");
    };
    //eo get.js

    //gui.js
    goosh.lib.namespace("goosh.gui");
    goosh.gui.ddate = false;
    goosh.gui.inputel = false;
    goosh.gui.outputel = false;
    goosh.gui.promptel = false;
    goosh.gui.inputfield = false;
    goosh.gui.bodyel = false;

    goosh.gui.el = function (id) {
        return document.getElementById(id);
    }

    goosh.gui.init = function () {

        goosh.gui.ddate = document.getElementById('ddate');
        goosh.gui.inputel = document.getElementById('input');
        goosh.gui.outputel = document.getElementById('output');
        goosh.gui.promptel = document.getElementById('prompt');
        goosh.gui.inputfield = document.getElementById('inputfield');
        goosh.gui.bodyel = document.getElementById('body');

        if (goosh.gui.inputfield.createTextRange) {
            goosh.gui.inputfield.onkeyup = new Function("return goosh.keyboard.mcursor(event);");
            goosh.gui.bodyel.onfocus = new Function("return goosh.gui.focusinput(event);");
            goosh.gui.bodyel.onclick = new Function("return goosh.gui.focusinput(event);");
            goosh.gui.bodyel.onkeydown = new Function("return goosh.keyboard.keyDownHandler(event);");
        }
        else {
            goosh.gui.inputfield.onkeyup = goosh.keyboard.mcursor;
            goosh.gui.bodyel.onfocus = goosh.gui.focusinput;
            goosh.gui.bodyel.onclick = goosh.gui.focusinput;
            goosh.gui.bodyel.onkeydown = goosh.keyboard.keyDownHandler;
        }

    };

    // input / output functions
    goosh.gui.error = function (A) { goosh.ajax.stopall(); goosh.gui.out("Error: " + A + "<br/> <br/>"); goosh.gui.showinput(); goosh.gui.focusinput(); goosh.gui.scroll(); };
    goosh.gui.outln = function (A) { goosh.gui.out(A + "<br/>"); };
    goosh.gui.out = function (A) { var B = document.createElement("div"); B.innerHTML = A; goosh.gui.outputel.appendChild(B); };
    goosh.gui.less = function (A) { return "<span class='less'>" + A + "</span>"; };
    goosh.gui.info = function (A) { return "<span class='info'>" + A + "</span>"; };
    goosh.gui.clear = function () { goosh.gui.outputel.innerHTML = "<br><br><br><br>"; };
    goosh.gui.showinput = function () { goosh.gui.inputel.style['display'] = 'block'; };
    goosh.gui.hideinput = function () { goosh.gui.inputel.style['display'] = 'none'; };
    goosh.gui.focusinput = function () { var A = ""; if (document.selection) A = document.selection.createRange().text; else if (window.getSelection) A = window.getSelection().toString(); if (A.length == 0) { document.f.q.value = document.f.q.value; if (goosh.gui.inputel.style['display'] != 'none') document.f.q.focus(); } };

    goosh.gui.updateprompt = function () {
        goosh.gui.prompt = goosh.config.user + "@" + goosh.config.host + " [" + goosh.config.mode + "-mode] " + goosh.config.pend;
        goosh.gui.promptel.innerHTML = goosh.gui.prompt;
    };



    goosh.gui.scroll = function () { window.scrollBy(0, 122500); };
    goosh.gui.setstyle = function (A, B, C) { try { var D = goosh.gui.el(A); D.style[B] = C; return true; } catch (e) { return false; } };
    goosh.gui.setstyleclass = function (A, B) { var C = document.createElement("div"); var D = "<br style='line-height:0px;'/><style>" + A + " {" + B + "}</style>"; C.innerHTML = D; goosh.gui.bodyel.appendChild(C); };
    //eo gui.js

    //set.js
    goosh.lib.namespace("goosh.set");
    goosh.set.base = function (A, B, C, D, E) { this.name = A; this.txt = C; this.def = B; (E) ? this.max = E : this.max = 2000; (D) ? this.min = D : this.min = 0; if (D && E) this.txt += " (" + D + ".." + E + ")"; this.get = function () { return eval("" + this.name + ";"); }; this.set = function (val) { if (val >= this.min && val <= this.max) eval("" + this.name + " = '" + val + "';"); return true; }; }
    goosh.set.list = new Object();
    goosh.set.list['lang'] = new goosh.set.base("goosh.config.lang", "en", "default language");
    goosh.set.list['results'] = new goosh.set.base("goosh.config.numres", "4", "number of results for google-searches", 1, 100);
    goosh.set.list['timeout'] = new goosh.set.base("goosh.config.timeout", "4", "timeout for ajax requests in seconds", 1, 100);
    goosh.set.list['style.bg'] = new goosh.set.base("goosh.config.bgcolor", "#FFFFFF", "goosh background color");
    goosh.set.list['style.bg'].set = function (val) {
        if (goosh.gui.setstyle("body", "backgroundColor", val) &&
      goosh.gui.setstyle("inputfield", "backgroundColor", val)) {
            goosh.config.bgcolor = val;
            return true;
        } else
            return false;
    }

    goosh.set.list['style.fg'] = new goosh.set.base("goosh.config.fgcolor", "#000000", "goosh font color");
    goosh.set.list['style.fg'].set = function (val) {
        if (goosh.gui.setstyle("body", "color", val) &&
            goosh.gui.setstyle("inputfield", "color", val)) {
            goosh.config.fgcolor = val;
            return true;
        } else
            return false;
    }

    goosh.set.list['style.hl'] = new goosh.set.base("goosh.config.hlcolor", "#009900", "goosh highlight color");
    goosh.set.list['style.hl'].set = function (val) {
        goosh.gui.setstyleclass(".info", "color: " + val);
        goosh.gui.setstyleclass("a:visited.info", "color: " + val);
        goosh.config.hlcolor = val;
        return true;
    }

    goosh.set.list['style.sh'] = new goosh.set.base("goosh.config.shcolor", "#666666", "goosh 'shaded' color");
    goosh.set.list['style.sh'].set = function (val) {
        goosh.gui.setstyleclass(".less", "color: " + val);
        goosh.config.shcolor = val;
        return true;
    }

    goosh.set.list['style.link'] = new goosh.set.base("goosh.config.linkcolor", "#0000CC", "goosh link color");
    goosh.set.list['style.link'].set = function (val) {
        goosh.gui.setstyleclass("a", "color: " + val);
        goosh.config.linkcolor = val;
        return true;
    }

    goosh.set.list['style.vlink'] = new goosh.set.base("goosh.config.vlinkcolor", "#551a8b", "goosh visited link color");
    goosh.set.list['style.vlink'].set = function (val) {
        goosh.gui.setstyleclass("a:visited", "color: " + val);
        goosh.config.vlinkcolor = val;
        return true;
    }

    goosh.set.list['place.width'] = new goosh.set.base("goosh.config.mapwidth", "300", "width of map image", 20, 600);
    goosh.set.list['place.height'] = new goosh.set.base("goosh.config.mapheight", "150", "height of map image", 20, 500);

    goosh.set.init = function (context, result) {
        goosh.gui.outln("Loading local settings...");

        for (key in goosh.set.list) {
            var c = false;
            if (c && goosh.set.list[key].set(c)) {
                goosh.gui.outln("&nbsp;" + key + " => &quot;" + c + "&quot;.");
            } else {
                goosh.set.list[key].set(goosh.set.list[key].def);
            }
        }
        goosh.gui.outln("");
        goosh.getquery();
    }
    //eo set.js

    //ajax.js
    goosh.lib.namespace("goosh.ajax");

    goosh.ajax.contexts = new Array();
    goosh.ajax.lastcontext = false;

    goosh.ajax.stopall = function () {
        for (key in goosh.ajax.contexts) {
            goosh.ajax.iscontext(key);
        }
    }

    goosh.ajax.deletecontext = function (context) {
        goosh.gui.outln('Error: Operation timed out. ' + context);
        if (!document.all) goosh.gui.outln(goosh.gui.less('If you use the noscript firefox-extension, add "ajax.googleapis.com" to the whitelist.'));
        goosh.gui.outln('');
        goosh.ajax.contexts[context] = false;

        var d = document.getElementById(context);
        if (d) document.body.removeChild(d);

        goosh.gui.showinput();
        goosh.gui.focusinput();
        goosh.gui.scroll();
        if (!document.all) stop();
    }

    goosh.ajax.iscontext = function (name) {
        if (goosh.ajax.contexts[name]) {
            clearTimeout(goosh.ajax.contexts[name]);
            goosh.ajax.contexts[name] = false;
            var d = document.getElementById(name);
            if (d) document.body.removeChild(d);
            return true;
        } else
            return false;
    }

    goosh.ajax.getcontext = function (name) {
        var d = new Date();
        var context = d.getTime();
        if (name) context = name;
        goosh.ajax.contexts[context] = setTimeout("goosh.ajax.deletecontext('" + context + "');", 1000 * goosh.config.timeout);
        return context;
    }

    goosh.ajax.query = function (url, nohide) {
        var context = "none";
        if (!nohide) {
            context = goosh.ajax.getcontext();
            goosh.ajax.lastcontext = context; // more elegant with return, but doesnt work in opera
            goosh.gui.hideinput();
        }
        var script = document.createElement("script");
        document.body.appendChild(script);
        script.src = url + '&context=' + context + '&';
        script.id = context;
    }
    //eo ajax.js

    //keyboard.js
    goosh.lib.namespace("goosh.keyboard");

    goosh.keyboard.suggestions = new Array();
    goosh.keyboard.suggpos = 1;
    goosh.keyboard.suggword = "";

    goosh.keyboard.hist = new Array();
    goosh.keyboard.histpos = 0;
    goosh.keyboard.histtemp = 0;


    goosh.keyboard.suggest = function (word) {
        if (goosh.keyboard.suggpos > goosh.keyboard.suggestions[word].length) goosh.keyboard.suggpos = 1;

        if (goosh.keyboard.suggestions[word][goosh.keyboard.suggpos])
            goosh.gui.inputfield.value = goosh.keyboard.suggestions[word][goosh.keyboard.suggpos];

        var d = goosh.gui.inputfield;
        if (d.createTextRange) {
            var t = d.createTextRange();
            t.moveStart("character", word.length);
            t.select()
        } else if (d.setSelectionRange) {
            d.setSelectionRange(word.length, d.value.length)
        }
    }


    // evil hack for suggest 
    goosh.keyboard.dummyac = function () {

        this.Suggest_apply = function (el, text, sug, temp) {

            goosh.keyboard.suggestions[text] = sug;
            goosh.keyboard.suggest(text);
            return true;
        }

    };

    window.google = new Array();

    window.google.ac = new goosh.keyboard.dummyac();


    goosh.keyboard.keyDownHandler = function (event) {
        if (!event && window.event) {
            event = window.event;
        }
        if (event) {
            _lastKeyCode = event.keyCode;
        }

        // We are backspacing here...
        if (event && event.keyCode == 9) {
            event.cancelBubble = true;
            event.returnValue = false;
            // tab = 9, backsp = 8, ctrl =17, r = 82
            //output.innerHTML += event.keyCode+"<br/>";

            var word = goosh.keyboard.suggword;

            if (word != "") {
                if (!goosh.keyboard.suggestions[word]) {
                    goosh.keyboard.suggpos = 1;
                    //	output.innerHTML += "query<br/>";
                    var script = document.createElement('script');
                    document.body.appendChild(script);
                    script.src = "http://www.google.com/complete/search?hl=" + goosh.config.lang + "&js=true&qu=" + encodeURIComponent(word);
                }
                else {
                    goosh.keyboard.suggpos += 2;
                    goosh.keyboard.suggest(word);
                }
            }
            return false
        }
    }



    goosh.keyboard.mcursor = function (e) {
        var keycode = e.keyCode;


        if (goosh.keyboard.hist.length > 0) {
            if (keycode == 38 || keycode == 40) {

                if (goosh.keyboard.hist[goosh.keyboard.histpos]) {
                    goosh.keyboard.hist[goosh.keyboard.histpos] = goosh.gui.inputfield.value;
                }
                else
                    goosh.keyboard.histtemp = goosh.gui.inputfield.value;
            }

            if (keycode == 38) { // up
                goosh.keyboard.histpos--;
                if (goosh.keyboard.histpos < 0) goosh.keyboard.histpos = 0;

            }
            else if (keycode == 40) { //down

                goosh.keyboard.histpos++;
                if (goosh.keyboard.histpos > goosh.keyboard.hist.length)
                    goosh.keyboard.histpos = goosh.keyboard.hist.length;
            }

            if (keycode == 38 || keycode == 40) {

                if (goosh.keyboard.hist[goosh.keyboard.histpos])
                    goosh.gui.inputfield.value = goosh.keyboard.hist[goosh.keyboard.histpos];
                else
                    goosh.gui.inputfield.value = goosh.keyboard.histtemp;

            }

        }

        if (keycode != 9 && keycode != 13)
            goosh.keyboard.suggword = goosh.gui.inputfield.value;

        if (keycode == 13) {
            goosh.command();
        }
    }
    //eo keyboard.js

    //modules.js
    goosh.lib.namespace("goosh.modules");
    goosh.lib.namespace("goosh.module");
    goosh.lib.namespace("goosh.modobj");

    //function yield(){
    //  if(cmdqueue.length >0)  command(cmdqueue.pop());
    //}


    goosh.modules.list = new Array();


    goosh.module.base = function () {

        this.mode = false;
        //  this.more = false;
        this.parameters = "";
        this.help = "no helptext yet.";
        this.helptext = "";
        this.hasmore = false;
        this.results = new Array();

    }

    goosh.modules.register = function (name, base) {
        if (!base) base = "base";
        eval(//"search_"+name+".prototype = new search_"+base+"();"+
        //"searchers_"+name+" = new search_"+name+"();"+
       'goosh.module.' + name + '.prototype = new goosh.module.' + base + ';' +
       'goosh.modobj.' + name + ' = new goosh.module.' + name + ';' +
       'goosh.modules.list["' + name + '"] = goosh.modobj.' + name + ";");
    }

    //load modules

    //intern help modul
    goosh.module.help = function () {

        this.name = "help";
        this.aliases = new Array("help", "man", "h", "?");

        this.help = "displays help text";
        this.helptext = "";
        this.parameters = "[command]";

        this.call = function (args) {

            if (args[0] == "goosh") args[0] = false;

            var out = "<span class='info'>help";
            if (args[0]) out += ": " + args[0];
            out += "</span><br/> <br/>";

            if (args[0] && !goosh.modules.list[args[0]]) {
                goosh.gui.error("command &quot;" + args[0] + "&quot; not found.");
                return false;
            }

            out += "<table border='0' class='help'>";
            out += "<tr><td class='less'>command</td><td class='less'>aliases</td><td class='less'>parameters</td><td class='less'>function</td></tr>";

            var module;

            for (key in goosh.modules.list) {
                if (!args[0] || key == args[0]) {
                    module = goosh.modules.list[key];

                    out += "<tr><td";
                    if (module.mode) out += " class='info'";
                    out += ">";
                    out += "" + module.name + "</td><td>";
                    if (module.aliases.length > 1) {
                        out += "(";
                        for (i = 0; i < module.aliases.length; i++) {
                            if (module.aliases[i] != module.name) {
                                out += module.aliases[i];
                                out += ",";
                            }
                        }

                        out = out.substr(0, out.length - 1);
                        out += ")";
                    }
                    out += "</td><td>";
                    if (module.parameters) out += module.parameters;
                    out += "</td><td>";
                    out += "" + module.help + "\n";
                    out += "</td></tr>";

                }
            }

            out += "</table>";

            if (args[0]) {
                out += " <br/>";
                out += module.helptext;
                out += " <br/>";
            }
            else {
                out += " <br/>";
                out += "- Aliases will expand to commands.<br/>";
                out += "- Use cursor up and down for command history.<br/>";
                out += "<br/>";
            }
            goosh.gui.outln(out);
        }
    }

    goosh.modules.register("help");
    //eo help module



    //clear module
    goosh.module.clear = function () {

        this.name = "clear";
        this.aliases = new Array("clear", "c");
        this.help = "clear the screen";

        this.call = function (args) {
            goosh.gui.outputel.innerHTML = "<br>";
        }

        goosh.module.help.call();
    }

    goosh.modules.register("clear");
    //eo clear module

    ////CLI Handler
    //goosh.module.cli = function() {
    //    this.name = "cli";
    //    this.aliases = new Array("cli", "c");
    //    this.help = "switch to cli mode";

    //    this.call = function(args) {
    //        if (goosh.config.mode != "cli") {
    //            goosh.config.mode = "cli";
    //            goosh.gui.updateprompt();
    //        } else {
    //            //handle arguments
    //            handleQuery(args);
    //        }
    //    }
    //}
    //goosh.modules.register("cli");
    //eo cli

    //format handler
    goosh.module.format = function () {
        getOutputFormats();

        this.name = "format";
        this.aliases = new Array("format");
        this.help = "sets the output-format  of query results default is " + goosh.config.webservice_default_format.name;

        var parameters = "[";
        $.each(goosh.config.webservice_formats, function (key, value) {
            if (key > 0) parameters += "|";
            parameters += value.name;
        });
        parameters += "]";
        this.parameters = parameters;

        this.call = function (args) {
            if ((args == undefined) || (args.length == 0)) {
                goosh.gui.out("current output format is set to: " + goosh.config.webservice_default_format.name);
            } else {
                for (i = 0; i < goosh.config.webservice_formats.length; i++) {
                    if (goosh.config.webservice_formats[i].name == args[0]) {
                        goosh.config.webservice_default_format = goosh.config.webservice_formats[i];
                        goosh.gui.out("current output format is now set to: " + goosh.config.webservice_default_format.name);
                        return 0;
                    }
                }
                goosh.gui.error("no valid parameter found. try one of these: " + this.parameters + "\"");
            }
        }
    }
    goosh.modules.register("format");

    //format option handler
    goosh.module.formatoption = function () {
        this.name = "formatoption";
        this.aliases = new Array("formatoption", "fo");
        this.help = "set option of current format plugin, list possible options when called without parameter";
        this.parameters = "[option=value]";

        this.call = function (args) {
            if ((args == undefined) || (args.length == 0)) {
                goosh.gui.out("list of options: bla, bla");
            } else {
                //build the target URI
                var target = goosh.config.webservice_protocol + "://"
                + goosh.config.webservice_host
                + ((goosh.config.webservice_port != undefined) ? (":" + goosh.config.webservice_port) : "")
                + "/"
                + goosh.config.webservice_path + "setformatparams"
                + "?" + args[0];

                //do some ajax
                var RESTResponse = $.ajax({
                    type: "POST",
                    url: target,
                    cache: false,
                    async: false,
                    timeout: 0,
                    error: function (xhr, ajaxOptions, thrownError) {
                        return ("AJAX Error " + xhr.status + "\n" + data.responseText + "\n" + thrownError);
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('Accept', goosh.config.webservice_default_format.type);
                    }
                });

                if (RESTResponse != null) {
                    goosh.gui.out(RESTResponse.responseText);
                }
            }
        }
    }
    goosh.modules.register("formatoption");

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
    //eo gql


    //goosh.js
    goosh.command = function () {
        //show waiting image
        goosh.gui.setWaiting(true);

        var cmdpar = goosh.gui.inputfield.value;
        var tokens = cmdpar.split(" ");
        var args = new Array();

        for (i = 0; i < tokens.length; i++) {
            if (tokens[i] != "") {
                if (tokens[0] != "set" && tokens[0] != "settings") {
                    var j = 1;
                    while (goosh.config.urls[j]) {  // replace search result numbers
                        if (tokens[i] == j) {
                            tokens[i] = goosh.config.urls[j];
                            if (i == 0) args.push("open"); // number shortcut
                        }
                        j++;
                    }
                }
                args.push(tokens[i]);
            }
        }
        var searcher;
        for (key in goosh.modules.list) {
            if (goosh.lib.in_array(goosh.modules.list[key].aliases, args[0])) {
                searcher = goosh.modules.list[key];
                args[0] = searcher.name;
                break;
            }
        }
        if (args.length == 0 && goosh.config.moreobj && goosh.config.moreobj.hasmore) {
            searcher = goosh.modules.list["more"];
            args[0] = "more";
        }

        var cmdstrnew = args.join(" ");
        if (encodeURIComponent(cmdstrnew) != goosh.lib.get("q") && cmdstrnew != "more" && cmdstrnew != "logout")
            window.location.hash = "#" + encodeURIComponent(cmdstrnew);

        goosh.gui.out("<div class='input'><span class='less'>" + goosh.gui.prompt + "</span>" + cmdstrnew.replace(/</g, "&lt;") + "</div>");
        if (cmdstrnew != "") {
            goosh.keyboard.hist[goosh.keyboard.hist.length] = cmdstrnew;
            goosh.keyboard.histpos = goosh.keyboard.hist.length;
        }
        var cmd = "";

        if (!searcher) {
            searcher = goosh.modules.list[goosh.config.mode]; // default searcher = mode
        } else {
            for (i = 0; i < args.length - 1; i++) args[i] = args[i + 1];
            args.pop();
        }

        //more
        if (searcher.more && args.length > 0) this.config.moreobj = searcher;

        if (args.length == 0 && searcher.mode) {
            goosh.config.mode = searcher.name;
            goosh.gui.updateprompt();
        } else {
            searcher.call(args);
        }
        goosh.gui.scroll();
        goosh.gui.inputfield.value = '';
        goosh.gui.focusinput();

        //hide waiting image
        goosh.gui.setWaiting(false);

        return false;
    }

    goosh.onload = function (e, username) {
        var ifrlogin = false;
        try {
            if (parent.goosh != goosh) {
                var bodyel = document.getElementById('body');
                bodyel.innerHTML = "";
                goosh = parent.goosh;
                ifrlogin = true;
            }
        }
        catch (e) { }

        goosh.gui.init();
        goosh.set.init();
        UpdateDDate();

    }

    goosh.getquery = function () {
        var query = ""
        if (goosh.lib.get("q")) {
            query = goosh.lib.get("q");
        }
        if (window.location.hash) {
            query = decodeURIComponent(window.location.hash.substr(1));
        }
        query += " ";

        if (query != " " && query.substr(0, 6) != "login " && query.substr(0, 4) != "set " && query.substr(0, 9) != "settings ")
            goosh.gui.inputfield.value = query.substr(0, query.length);
        else goosh.gui.inputfield.value = "";

        goosh.gui.updateprompt();
        goosh.gui.showinput();
        goosh.gui.focusinput();

        if (goosh.gui.inputfield.value != "") {
            goosh.command();
        }

    }

    return goosh;

}

function getOutputFormats() {
    //build the target URI
    var target = goosh.config.webservice_protocol + "://"
                + goosh.config.webservice_host
                + ((goosh.config.webservice_port != undefined) ? (":" + goosh.config.webservice_port) : "")
                + "/"
                + goosh.config.webservice_path + "availableoutputformats";

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
        var formats = $.parseJSON(html.responseText);
        $.each(formats, function (key, val) {
            if (key == "GraphDSOutputFormats") {
                $.each(val, function (key, val) {
                    $.each(val, function (key, val) {
                        if (val == "json") {
                            goosh.config.webservice_default_format.name = val;
                            goosh.config.webservice_default_format.type = key;
                        }
                        goosh.config.webservice_formats.push({ name: val, type: key });
                    });
                });
            }
        });
        if (goosh.config.webservice_default_format.name == "not set") {
            goosh.config.webservice_default_format = goosh.config.webservice_formats[0];
        }
        return 0;
    }
}