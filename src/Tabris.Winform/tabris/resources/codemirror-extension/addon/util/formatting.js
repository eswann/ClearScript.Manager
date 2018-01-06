(function () {

    CodeMirror.extendMode("css", {
        commentStart: "/*",
        commentEnd: "*/",
        lineCommentStart: "/*",
        lineCommentEnd: "*/",
        newlineAfterToken: function (type, content) {
            return /^[;{}]$/.test(content);
        }
    });

    CodeMirror.extendMode("javascript", {
        commentStart: "/*",
        commentEnd: "*/",
        lineCommentStart: "//",
        lineCommentEnd: "",
        // FIXME semicolons inside of for
        newlineAfterToken: function (type, content, textAfter, state) {
            if (this.jsonMode) {
                return /^[\[,{]$/.test(content) || /^}/.test(textAfter);
            } else {
                if (content == ";" && state.lexical && state.lexical.type == ")") return false;
                return /^[;{}]$/.test(content) && !/^;/.test(textAfter);
            }
        }
    });

    CodeMirror.extendMode("xml", {
        commentStart: "<!--",
        commentEnd: "-->",
        lineCommentStart: "<!--",
        lineCommentEnd: "-->",
        newlineAfterToken: function (type, content, textAfter) {
            return type == "tag" && />$/.test(content) || /^</.test(textAfter);
        }
    });

    // Comment/uncomment the specified range
    CodeMirror.defineExtension("commentRange", function (isComment, from, to) {
        var cm = this, curMode = CodeMirror.innerMode(cm.getMode(), cm.getTokenAt(from).state).mode;
        cm.operation(function () {
            var commentEnd = curMode.commentEnd;
            var commentStart = curMode.commentStart;
           
            if (from.line == to.line) {
                commentEnd = curMode.lineCommentEnd;
                commentStart = curMode.lineCommentStart;

                if (from.ch == to.ch) {
                    to = { line: to.line, ch: to.ch, xRel: to.xRel };
                    var allWorldLength = cm.getLine(to.line).length;
                    var length = allWorldLength - cm.getLine(to.line).trim().length;
                    if (isComment) {
                        from.ch = length;
                    } else {
                        from.ch = (length - 1) < 0 ? 0 : (length - 1);
                        to.ch = allWorldLength;
                    }
                   
                }
            }
            if (isComment) { // Comment range
                cm.replaceRange(commentEnd, to);
                cm.replaceRange(commentStart, from);
                if (from.line == to.line) // An empty comment inserted - put cursor inside
                    cm.setCursor(from.line, from.ch + commentStart.length);
            } else { // Uncomment range
                debugger
                var selText = cm.getRange(from, to);
                var startIndex = selText.indexOf(commentStart);
                var endIndex = selText.lastIndexOf(commentEnd);
                if (startIndex > -1 && endIndex > -1 && endIndex > startIndex) {
                    // Take string till comment start
                    selText = selText.substr(0, startIndex)
                    // From comment start till comment end
                      + selText.substring(startIndex + commentStart.length, endIndex)
                    // From comment end till string end
                      + selText.substr(endIndex + commentEnd.length);
                }
                cm.replaceRange(selText, from, to);
            }
        });
    });

    // Applies automatic mode-aware indentation to the specified range
    CodeMirror.defineExtension("autoIndentRange", function (from, to) {
        var cmInstance = this;
        this.operation(function () {
            for (var i = from.line; i <= to.line; i++) {
                cmInstance.indentLine(i, "smart");
            }
        });
    });

    // Applies automatic formatting to the specified range
    CodeMirror.defineExtension("autoFormatRange", function (from, to) {
        var cm = this;
        var outer = cm.getMode(), text = cm.getRange(from, to).split("\n");
        var state = CodeMirror.copyState(outer, cm.getTokenAt(from).state);
        var tabSize = cm.getOption("tabSize");

        var out = "", lines = 0, atSol = from.ch == 0;
        function newline() {
            out += "\n";
            atSol = true;
            ++lines;
        }

        for (var i = 0; i < text.length; ++i) {
            var stream = new CodeMirror.StringStream(text[i], tabSize);
            while (!stream.eol()) {
                var inner = CodeMirror.innerMode(outer, state);
                var style = outer.token(stream, state), cur = stream.current();
                stream.start = stream.pos;
                if (!atSol || /\S/.test(cur)) {
                    out += cur;
                    atSol = false;
                }
                if (!atSol && inner.mode.newlineAfterToken &&
                    inner.mode.newlineAfterToken(style, cur, stream.string.slice(stream.pos) || text[i + 1] || "", inner.state))
                    newline();
            }
            if (!stream.pos && outer.blankLine) outer.blankLine(state);
            if (!atSol) newline();
        }

        cm.operation(function () {
            cm.replaceRange(out, from, to);
            for (var cur = from.line + 1, end = from.line + lines; cur <= end; ++cur)
                cm.indentLine(cur, "smart");
            cm.setSelection(from, cm.getCursor(false));
        });
    });
})();
