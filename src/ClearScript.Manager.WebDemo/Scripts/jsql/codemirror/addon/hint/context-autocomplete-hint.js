(function(mod) {
  if (typeof exports == "object" && typeof module == "object") // CommonJS
    mod(require("../../lib/codemirror"));
  else if (typeof define == "function" && define.amd) // AMD
    define(["../../lib/codemirror"], mod);
  else // Plain browser env
    mod(CodeMirror);
})(function(CodeMirror) {
  "use strict";

  // Note: the string autocompletion works for the first function argument.
  CodeMirror.commands.contextAutocomplete = function(cm) {
    var contextAutocompleteFunctionArgs = {
      "tabris.create": "type",
      "set": "name",
      "get": "name",
      "on": "type",
      "off": "type",
      "trigger": "type"
    };
    var joinedRegexes = getJoinedRegexes(contextAutocompleteFunctionArgs);
    var args = getAutocompleteFunctionArgs(contextAutocompleteFunctionArgs);
    processVariableForMatchedFunctionArg(cm, args, joinedRegexes, contextAutocompleteFunctionArgs);
    setTimeout(function() {
      CodeMirror.showHint(cm, CodeMirror.ternHint, {async: true});
    }, 0);
  };

  function createMatchRegexForFunction(func, arg) {
    var regex;
    regex = func.replace(".", "\\.");
    regex += "\\(";
    regex += createArgumentMatchRegexPartFor(arg);
    return regex;
  }

  function createArgumentMatchRegexPartFor(arg) {
    var regex = "(";
    for (var i = 0; i < arg.length; i++) {
      regex += (i !== 0 ? "|" : "") + arg.substring(0, i + 1);
    }
    regex += ")$";
    return regex;
  }

  function joinRegexes(regexArr) {
    var joinedRegex = "";
    var counter = 0;
    regexArr.forEach(function(arg) {
      joinedRegex += (counter++ !== 0 ? "|" : "") + "(" + arg + ")";
    });
    return joinedRegex;
  }

  function getJoinedRegexes(contextAutocompleteFunctionArgs) {
    var regexArr = [];
    for (var func in contextAutocompleteFunctionArgs) {
      regexArr.push(createMatchRegexForFunction(func, contextAutocompleteFunctionArgs[func]));
    }
    return joinRegexes(regexArr);
  }

  function getAutocompleteFunctionArgs(contextAutocompleteFunctionArgs) {
    var args = [];
    for (var func in contextAutocompleteFunctionArgs) {
      args.push(contextAutocompleteFunctionArgs[func]);
    }
    return args;
  }

  function deselectSelection(cm, isMatch) {
    var cursor = cm.getCursor();
    if (isMatch) {
      cursor.ch -= 1;
    }
    cm.setSelection(cursor);
  }

  function processVariableForMatchedFunctionArg(cm, args, joinedRegexes) {
    var cursor = cm.getCursor();
    var selection = cm.getSelection();
    var wordUnderCursor = selection !== "" ? selection : cm.getTokenAt(cursor).string;
    var wordUnderCursorIsArg = args.indexOf(wordUnderCursor) > -1;
    var rightLeftSelection = cm.getLine(cursor.line)[cursor.ch - 1] === "(" && wordUnderCursorIsArg;
    var line = cm.getLine(cursor.line).substring(0,
      rightLeftSelection ? cursor.ch  + selection.length : cursor.ch);
    var isMatch = line.match(joinedRegexes) && wordUnderCursorIsArg;
    if (isMatch) {
      if (selection === "") {
        cm.execCommand("goGroupLeft");
        cm.execCommand("delWordAfter");
      }
      cm.replaceSelection("\"\"");
    }
    deselectSelection(cm, isMatch);
  }

});
