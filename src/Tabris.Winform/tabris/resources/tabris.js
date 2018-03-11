(function (mod) {
    if (typeof exports == "object" && typeof module == "object") { // CommonJS
        return mod(require("tern/lib/infer"), require("tern/lib/tern"));
    }
    if (typeof define == "function" && define.amd) // AMD
        return define(["tern/lib/infer", "tern/lib/tern"], mod);
    mod(tern, tern);
})(function (infer, tern) {
    "use strict";

    var defaultRules = {
        "UnknownTabrisType": { "severity": "error" },
        "UnknownTabrisProperty": { "severity": "error" },
        "UnknownTabrisEvent": { "severity": "error" }
    };

    if (tern.registerLint) {

        // validate tabris.create(
        tern.registerLint("tabrisCreate_lint", function (node, addMessage, getRule) {
            var argNode = node.arguments[0];
            if (argNode) {
                var cx = infer.cx(), types = cx.definitions.tabris["!types"], typeName = argNode.value;
                if (!types.hasProp(typeName)) addMessage(argNode, "Unknown tabris type '" + typeName + "'", defaultRules.UnknownTabrisType.severity);
            }
        });

        // validate widget.get(
        tern.registerLint("tabrisGet_lint", function (node, addMessage, getRule) {
            var argNode = node.arguments[0];
            if (argNode) {
                var cx = infer.cx(), proxyType = argNode._tabris && argNode._tabris.proxyType, propertyName = argNode.value;
                if (!getPropertyType(proxyType, propertyName)) addMessage(argNode, "Unknown tabris property '" + propertyName + "'", defaultRules.UnknownTabrisProperty.severity);
            }
        });

        // validate on, off, trigger event(
        tern.registerLint("tabrisEvent_lint", function (node, addMessage, getRule) {
            var argNode = node.arguments[0];
            if (argNode) {
                var cx = infer.cx(), proxyType = argNode._tabris && argNode._tabris.proxyType, eventName = argNode.value;
                if (!getEventType(proxyType, eventName)) addMessage(argNode, "Unknown tabris event '" + eventName + "'", defaultRules.UnknownTabrisEvent.severity);
            }
        });

    }

    // tabris.create(

    infer.registerFunction("tabris_create", function (_self, args, argNodes) {
        if (!argNodes || !argNodes.length || argNodes[0].type != "Literal" || typeof argNodes[0].value != "string")
            return infer.ANull;
        var cx = infer.cx(), server = cx.parent, name = argNodes[0].value, locals = cx.definitions.tabris["!types"], tabrisType = locals.hasProp(name);
        argNodes[0]._tabris = { "type": "tabris_create" };
        if (tabrisType) return new infer.Obj(tabrisType.getType().getProp("prototype").getType());
        return infer.ANull;
    });

    // widget.get(

    function getObjectProperties(proto) {
        var cx = infer.cx(), locals = cx.definitions.tabris;
        var objectName = proto.name, index = objectName.indexOf("!types.");
        if (index == 0) objectName = objectName.substring("!types.".length, objectName.length);
        objectName = objectName.substring(0, objectName.indexOf('.')) + 'Properties';
        return locals["!properties"].hasProp(objectName);
    }

    function getPropertyType(widgetType, propertyName) {
        if (!(widgetType)) return null;
        var proto = widgetType.proto, propertyType = null;
        while (proto) {
            var objectType = getObjectProperties(proto);
            if (objectType && objectType.getType) propertyType = objectType.getType().hasProp(propertyName)
            if (propertyType) return propertyType;
            proto = proto.proto;
        }
        return null;
    }

    ["tabris_Proxy_get", "tabris_Proxy_set"].forEach(function (name) {
        infer.registerFunction(name, function (_self, args, argNodes) {
            if (!argNodes || !argNodes.length || argNodes[0].type != "Literal" || typeof argNodes[0].value != "string")
                return infer.ANull;

            var widgetType = _self.getType(), propertyName = argNodes[0].value, propertyType = getPropertyType(widgetType, propertyName);
            argNodes[0]._tabris = { "type": "tabris_Proxy_get", "proxyType": widgetType };
            if (propertyType) return propertyType.getType();
            return infer.ANull;
        });
    });

    // widget.on(

    function getEventProperties(proto) {
        var cx = infer.cx(), locals = cx.definitions.tabris;
        var objectName = proto.name, index = objectName.indexOf("!types.");
        if (index == 0) objectName = objectName.substring("!types.".length, objectName.length);
        objectName = objectName.substring(0, objectName.indexOf('.')) + 'Events';
        return locals["!events"].hasProp(objectName);
    }

    function getEventType(widgetType, eventName) {
        if (!(widgetType)) return null;
        var proto = widgetType.proto, eventType = null;
        while (proto) {
            var objectType = getEventProperties(proto);
            if (objectType && objectType.getType) eventType = objectType.getType().hasProp(eventName)
            if (eventType) return eventType;
            proto = proto.proto;
        }
        return null;
    }

    infer.registerFunction("tabris_Proxy_eventtype", function (_self, args, argNodes) {
        if (!argNodes || !argNodes.length || argNodes[0].type != "Literal" || typeof argNodes[0].value != "string")
            return infer.ANull;

        var proxyType = _self.getType();
        argNodes[0]._tabris = { "type": "tabris_Proxy_eventtype", "proxyType": proxyType };
    });

    tern.registerPlugin("tabris", function (server, options) {
        return {
            defs: defs,
            passes: { completion: completion }
        };
    });

    function completion(file, query) {
        function getQuote(c) {
            return c === '\'' || c === '"' ? c : null;
        }

        if (!query.end) return; // remove this line, once tern will be released

        var wordPos = tern.resolvePos(file, query.end);
        var word = null, completions = [];
        var wrapAsObjs = query.types || query.depths || query.docs || query.urls || query.origins;
        var cx = infer.cx(), overrideType = null;

        function gather(prop, obj, depth, addInfo) {
            // 'hasOwnProperty' and such are usually just noise, leave them
            // out when no prefix is provided.
            if (obj == cx.protos.Object && !word) return;
            if (query.filter !== false && word &&
                (query.caseInsensitive ? prop.toLowerCase() : prop).indexOf(word) !== 0) return;
            for (var i = 0; i < completions.length; ++i) {
                var c = completions[i];
                if ((wrapAsObjs ? c.name : c) == prop) return;
            }
            var rec = wrapAsObjs ? { name: prop } : prop;
            completions.push(rec);

            if (obj && (query.types || query.docs || query.urls || query.origins)) {
                var val = obj.props[prop];
                infer.resetGuessing();
                var type = val.getType();
                rec.guess = infer.didGuess();
                if (query.types)
                    rec.type = overrideType != null ? overrideType : infer.toString(type);
                if (query.docs)
                    maybeSet(rec, "doc", val.doc || type && type.doc);
                if (query.urls)
                    maybeSet(rec, "url", val.url || type && type.url);
                if (query.origins)
                    maybeSet(rec, "origin", val.origin || type && type.origin);
            }
            if (query.depths) rec.depth = depth;
            if (wrapAsObjs && addInfo) addInfo(rec);
        }

        var callExpr = infer.findExpressionAround(file.ast, null, wordPos, file.scope, "CallExpression");
        if (callExpr && callExpr.node.arguments && callExpr.node.arguments.length && callExpr.node.arguments.length > 0) {
            var nodeArg = callExpr.node.arguments[0];
            if (!(nodeArg.start <= wordPos && nodeArg.end >= wordPos)) return;
            if (nodeArg._tabris) {
                var startQuote = getQuote(nodeArg.raw.charAt(0)), endQuote = getQuote(nodeArg.raw.length > 1 ? nodeArg.raw.charAt(nodeArg.raw.length - 1) : null);
                var wordEnd = endQuote != null ? nodeArg.end - 1 : nodeArg.end, wordStart = startQuote != null ? nodeArg.start + 1 : nodeArg.start,
                word = nodeArg.value.slice(0, nodeArg.value.length - (wordEnd - wordPos));
                if (query.caseInsensitive) word = word.toLowerCase();

                switch (nodeArg._tabris.type) {
                    case "tabris_Proxy_get":
                    case "tabris_Proxy_set":
                        var widgetType = nodeArg._tabris.proxyType, proto = widgetType.proto, propertyType = null;
                        while (proto) {
                            var objType = getObjectProperties(proto);
                            if (objType) infer.forAllPropertiesOf(objType, gather);
                            proto = proto.proto;
                        }
                        break;
                    case "tabris_Proxy_eventtype":
                        var widgetType = nodeArg._tabris.proxyType, proto = widgetType.proto, propertyType = null;
                        while (proto) {
                            var objType = getEventProperties(proto);
                            if (objType) infer.forAllPropertiesOf(objType, gather);
                            proto = proto.proto;
                        }
                        break;
                    case "tabris_create":
                        var types = cx.definitions.tabris["!types"];
                        overrideType = "string";
                        infer.forAllPropertiesOf(types, gather);
                        break;
                }

                return {
                    start: tern.outputPos(query, file, wordStart),
                    end: tern.outputPos(query, file, wordEnd),
                    isProperty: false,
                    isStringAround: true,
                    startQuote: startQuote,
                    endQuote: endQuote,
                    completions: completions
                }
            }
        }
    }

    function maybeSet(obj, prop, val) {
        if (val != null) obj[prop] = val;
    }

    var defs = {
        "!name": "tabris",
        "!define": {
            "!properties": {
                "LOGProperties": {
                    "trace": {
                        "!type": "bool",
                        "!doc": "Is need to append trace log"
                    }
                },
                "SQLProperties": {
                    "name": {
                        "!type": "string",
                        "!doc": "The db mapping name in config file."
                    },
                    "type": {
                        "!type": "string",
                        "!doc": "The db type(mysql or sqlserver)."
                    },
                    "connection": {
                        "!type": "string",
                        "!doc": "The connection string of db."
                    },
                    "timeout": {
                        "!type": "number",
                        "!doc": "The timeout of db excutor."
                    }
                },
                "HTTPProperties": {
                    "url": {
                        "!type": "string",
                        "!doc": "The request url."
                    },
                    "method": {
                        "!type": "string",
                        "!doc": "The request method(get or post).default is get"
                    },
                    "timeout": {
                        "!type": "number",
                        "!doc": "The request timeout(senconds)."
                    },
                    "accept": {
                        "!type": "string",
                        "!doc": "The request type(application/json or x-www-form-urlencoded)."
                    },
                    "headers": {
                        "!type": "properties",
                        "!doc": "The request headers(key-value)."
                    },
                    "cookieContainer": {
                        "!type": "properties",
                        "!doc": "The request cookieContainer"
                    }
                },
                "VIEWProperties": {
                    "url": {
                        "!type": "string",
                        "!doc": "The request url."
                    }
                }
            },
            "!events": {

            },
            "!types": {
                "VIEW": {
                    "!type": "fn()",
                    "!url": "",
                    "!doc": "VIEW Function .",
                    "prototype": {
                        "show": {
                            "!type": "fn(listener: fn()) -> ?",
                            "!doc": "create and show webview "
                        },
                        "on": {
                            "!type": "fn(name: string, listener: fn()) -> ?",
                            "!doc": "add callback event"
                        },
                        "getInitCookieString": {
                            "!type": "fn() -> string",
                            "!doc": "get the first initialized CookieString"
                        },
                        "getInitCookieContainer": {
                            "!type": "fn() -> ?",
                            "!doc": "get the first initialized CookieContainer"
                        },
                        "execJs": {
                            "!type": "fn(js: string) -> +AwaitString",
                            "!doc": "Execute js in the webview"
                        },
                        "getDomHtml": {
                            "!type": "fn() -> +AwaitString",
                            "!doc": "get all domString of the current webview"
                        },
                        "createTimer": {
                            "!type": "fn(interval: number) -> +Timer",
                            "!doc": "create a timer,inerval (millisecond)."
                        }
                    }
                },
                "SQL": {
                    "!type": "fn()",
                    "!url": "",
                    "!doc": "SQL Function .",
                    "prototype": {
                        "exec": {
                            "!type": "fn(sql: string, options?: ?) -> ?",
                            "!doc": "Execute sql.Automatically find the type of execution"
                        },
                        "insertWithIdentity": {
                            "!type": "fn(sql: string, options?: ?) -> string",
                            "!doc": "Execute insertWithIdentity sql"
                        },
                        "insert": {
                            "!type": "fn(sql: string, options?: ?) -> number",
                            "!doc": "Execute insert sql"
                        },
                        "update": {
                            "!type": "fn(sql: string, options?: ?) -> number",
                            "!doc": "Execute update sql"
                        },
                        "delete": {
                            "!type": "fn(sql: string, options?: ?) -> number",
                            "!doc": "Execute delete sql"
                        },
                        "query": {
                            "!type": "fn(sql: string, options?: ?) -> ?",
                            "!doc": "Execute query sql"
                        },
                        "useTransaction": {
                            "!type": "fn(func: fn(), options?: ?) -> ?",
                            "!doc": "Execute sql in transaction"
                        }
                    }
                },
                "HTTP": {
                    "!type": "fn()",
                    "!url": "",
                    "!doc": "HTTP Function .",
                    "prototype": {
                        "getString": {
                            "!type": "fn(options?: ?) -> string",
                            "!doc": "Execute httpr.request, return result as string"
                        },
                        "getJson": {
                            "!type": "fn(options?: ?) -> ?",
                            "!doc": "Execute httpr.request, return result as json object"
                        }
                    }
                },
                "LOG": {
                    "!type": "fn()",
                    "!url": "",
                    "!doc": "LOG Function .",
                    "prototype": {
                        "info": {
                            "!type": "fn(msg: string) -> ?",
                            "!doc": "Execute info log"
                        },
                        "warn": {
                            "!type": "fn(msg: string) -> ?",
                            "!doc": "Execute warn log"
                        },
                        "error": {
                            "!type": "fn(msg: string) -> ?",
                            "!doc": "Execute warn log"
                        },
                        "debug": {
                            "!type": "fn(msg: string) -> ?",
                            "!doc": "Execute warn log"
                        }
                    }
                }
            }
        },
        "tabris": {
            "create": {
                "!type": "fn(type: string, properties?: ?) -> !custom:tabris_create",
                "!doc": "Creates a instance of a given type.",
                "!url": "",
                "!data": {
                    "!lint": "tabrisCreate_lint"
                }
            }
        }
    };
});
