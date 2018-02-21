// CodeMirror, copyright (c) by Marijn Haverbeke and others
// Distributed under an MIT license: http://codemirror.net/LICENSE

(function (mod) {
    if (typeof exports == "object" && typeof module == "object") // CommonJS
        mod(require("../../lib/codemirror"));
    else if (typeof define == "function" && define.amd) // AMD
        define(["../../lib/codemirror"], mod);
    else // Plain browser env
        mod(CodeMirror);
})(function (CodeMirror) {
    "use strict";

    var HINT_ELEMENT_CLASS = "CodeMirror-hint";
    var ACTIVE_HINT_ELEMENT_CLASS = "CodeMirror-hint-active";

    // This is the old interface, kept around for now to stay
    // backwards-compatible.
    CodeMirror.showHint = function (cm, getHints, options) {
        if (!getHints) return cm.showHint(options);
        if (options && options.async) getHints.async = true;
        var newOpts = { hint: getHints };
        if (options) for (var prop in options) newOpts[prop] = options[prop];
        return cm.showHint(newOpts);
    };

    CodeMirror.defineExtension("showHint", function (options) {
        if(options && options.isFunc){
            if (this.state.completionActive) this.state.completionActive.close();
            var completion = this.state.completionActive = new Completion(this, options);
            CodeMirror.signal(this, "startCompletion", this);
            var items = [];
            var lineNumber=-1;
            for(var item in CodeMirror.functionTempList)
            {
                var line = CodeMirror.functionTempList[item];
                var lineText = this.getLineHandle(line).text;
                if(lineText.indexOf(item)>=0){
                    if(lineNumber == line) continue;
                    items.push(CodeMirror.functionList[line]);
                    lineNumber = line;
                }
            }
            if(items.length<1)return;
            return completion.showHints({isFunc:true,list:items,from:this.getCursor(true),to:this.getCursor(true)});
        }

        if(options && options.isQuick){
            if (this.state.completionActive) this.state.completionActive.close();
            var completion = this.state.completionActive = new Completion(this, options);
            CodeMirror.signal(this, "startCompletion", this);
            return completion.showHints({isQuick:true,list:options.data,from:this.getCursor(true),to:this.getCursor(true)});
        }
        // We want a single cursor position.
        if (this.listSelections().length > 1 || this.somethingSelected()) return;

        if (this.state.completionActive) this.state.completionActive.close();
        var completion = this.state.completionActive = new Completion(this, options);
        var getHints = completion.options.hint;
        if (!getHints) return;

        CodeMirror.signal(this, "startCompletion", this);
        if (getHints.async)
            getHints(this, function (hints) { completion.showHints(hints); }, completion.options);
        else
            return completion.showHints(getHints(this, completion.options));
    });

    function Completion(cm, options) {
        this.cm = cm;
        this.options = this.buildOptions(options);
        this.widget = this.onClose = null;
    }

    Completion.prototype = {
        close: function () {
            if (!this.active()) return;
            this.cm.state.completionActive = null;

            if (this.widget) this.widget.close();
            if (this.onClose) this.onClose();
            CodeMirror.signal(this.cm, "endCompletion", this.cm);
        },

        active: function () {
            return this.cm.state.completionActive == this;
        },

        pick: function (data, i) {
            var that = this;
            var completion = data.list[i];
            if(completion.range){
                that.cm.setCursor(Number(completion.range.end.line), Number(completion.range.end.ch));
                return;
            }
            function getSpaceText(num) {
                var ss = '';
                for(var yy=0;yy<num;yy++){
                    ss += ' ';
                }
                return ss;
            }

            function splitAndJoin(text,num,num2) {
                var aa = text.split('\n');
                var arr = [];
                for(var ii=0;ii<aa.length;ii++){
                    if(ii==0){
                        if(num2>0){
                            arr.push(getSpaceText(num2) + aa[ii]);continue;
                        }else{
                            arr.push(aa[ii]);continue;
                        }

                    }
                    arr.push(getSpaceText(num)+aa[ii]);
                }

                return arr.join('\n');
            }
            var replaceText = '';
            if(completion.selection && completion.temp){
                var globalIdx =completion.selection.head.ch;
                var startLine = that.cm.getLine(completion.selection.head.line);
                var temp1 = 0;
                for(var iy=0;iy<startLine.length;iy++){
                    if(startLine[iy] == ' '){
                        temp1++;
                    }else{
                        break;
                    }
                }
                var firstLineIdx =temp1-completion.selection.head.ch;
                if(firstLineIdx>0){
                    completion.selection.head.ch = firstLineIdx;
                }
                var selectText = that.cm.getRange(completion.selection.head,completion.selection.anchor);
                for(var y=0;y<completion.temp.length;y++){
                    var tt =completion.temp[y];
                    if(y==0){
                        replaceText += tt.text + '\n';
                        continue;
                    }
                    replaceText +=  getSpaceText(tt.idx+globalIdx);
                    if(tt.text == '$'){
                        replaceText +=   splitAndJoin(selectText,tt.idx,firstLineIdx) ;
                    }else{
                        replaceText += (firstLineIdx>0 ? getSpaceText(firstLineIdx):'') + tt.text;
                    }
                    if(y!=completion.temp.length-1){
                        replaceText += '\n';
                    }
                }

            }
            if (completion.hint) {
                completion.hint(that.cm, data, completion);
            }else if(replaceText.length>0){
                that.cm.replaceRange(replaceText, completion.selection.head,
                    completion.selection.anchor, "complete");
            }
            else {
                that.cm.replaceRange(getText(completion), completion.from || data.from,
                    completion.to || data.to, "complete");
            }
            CodeMirror.signal(data, "pick", completion);
            if (completion.template &&
                completion.template.name == completion.template.description &&
                completion.template.description == completion.template.template) {
                setTimeout(function () {
                    if (that.widget) that.widget.close();
                }, 1000);
            }
            that.close();
        },

        showHints: function (data) {
            if (!data || !data.list.length || !this.active()) return this.close();

            if (this.options.completeSingle && data.list.length == 1)
                this.showWidget(data);//this.pick(data, 0);
            else
                this.showWidget(data);
        },

        showWidget: function (data) {
            this.widget = new Widget(this, data);
            CodeMirror.signal(data, "shown");

            var debounce = 0, completion = this, finished;
            var closeOn = this.options.closeCharacters;
            var startPos = this.cm.getCursor(), startLen = this.cm.getLine(startPos.line).length;

            var requestAnimationFrame = window.requestAnimationFrame || function (fn) {
                return setTimeout(fn, 1000 / 60);
            };
            var cancelAnimationFrame = window.cancelAnimationFrame || clearTimeout;

            function done() {
                if (finished) return;
                finished = true;
                completion.close();
                completion.cm.off("cursorActivity", activity);
                if (completion.cm.tabrisAutoComplete) completion.cm.off("cursorActivity", completion.cm.tabrisAutoComplete);
                if (data) CodeMirror.signal(data, "close");
            }

            function update() {
                if (finished) return;
                CodeMirror.signal(data, "update");
                var getHints = completion.options.hint;
                if (getHints.async)
                    getHints(completion.cm, finishUpdate, completion.options);
                else
                    finishUpdate(getHints(completion.cm, completion.options));
            }
            function finishUpdate(data_) {
                data = data_;
                if (finished) return;
                if (!data || !data.list.length) return done();
                if (completion.widget) completion.widget.close();
                completion.widget = new Widget(completion, data);
            }

            function clearDebounce() {
                if (debounce) {
                    cancelAnimationFrame(debounce);
                    debounce = 0;
                }
            }

            function activity() {
                clearDebounce();
                var pos = completion.cm.getCursor(), line = completion.cm.getLine(pos.line);
                if (pos.line != startPos.line || line.length - pos.ch != startLen - startPos.ch ||
                    pos.ch < startPos.ch || completion.cm.somethingSelected() ||
                    (pos.ch && closeOn.test(line.charAt(pos.ch - 1)))) {
                    completion.close();
                } else {
                    debounce = requestAnimationFrame(update);
                    if (completion.widget) {
                        completion.widget.close();
                    }
                }
            }
            this.cm.on("cursorActivity", activity);
            this.onClose = done;
        },

        buildOptions: function (options) {
            var editor = this.cm.options.hintOptions;
            var out = {};
            for (var prop in defaultOptions) out[prop] = defaultOptions[prop];
            if (editor) for (var prop in editor)
                if (editor[prop] !== undefined) out[prop] = editor[prop];
            if (options) for (var prop in options)
                if (options[prop] !== undefined) out[prop] = options[prop];
            return out;
        }
    };

    function getText(completion) {
        if (typeof completion == "string") return completion;
        else return completion.text;
    }

    function buildKeyMap(completion, handle) {
        var baseMap = {
            Up: function () { handle.moveFocus(-1); },
            Down: function () { handle.moveFocus(1); },
            PageUp: function () { handle.moveFocus(-handle.menuSize() + 1, true); },
            PageDown: function () { handle.moveFocus(handle.menuSize() - 1, true); },
            Home: function () { handle.setFocus(0); },
            End: function () { handle.setFocus(handle.length - 1); },
            Enter: handle.pick,
            Tab: handle.pick,
            Esc: handle.close
        };
        var custom = completion.options.customKeys;
        var ourMap = custom ? {} : baseMap;
        function addBinding(key, val) {
            var bound;
            if (typeof val != "string")
                bound = function (cm) { return val(cm, handle); };
            // This mechanism is deprecated
            else if (baseMap.hasOwnProperty(val))
                bound = baseMap[val];
            else
                bound = val;
            ourMap[key] = bound;
        }
        if (custom)
            for (var key in custom) if (custom.hasOwnProperty(key))
                addBinding(key, custom[key]);
        var extra = completion.options.extraKeys;
        if (extra)
            for (var key in extra) if (extra.hasOwnProperty(key))
                addBinding(key, extra[key]);
        return ourMap;
    }

    function getHintElement(hintsElement, el) {
        while (el && el != hintsElement) {
            if (el.nodeName.toUpperCase() === "LI" && el.parentNode == hintsElement) return el;
            el = el.parentNode;
        }
    }

    function Widget(completion, data) {
        this.completion = completion;
        this.data = data;
        var widget = this, cm = completion.cm;

        var hints = this.hints = document.createElement("ul");
        hints.className = "CodeMirror-hints";
        this.selectedHint = data.selectedHint || 0;
        var inp;
        if(data.isFunc){

            inp = hints.appendChild(document.createElement("input"))
            inp.className = 'CodeMirror-hint-input';
            inp.hintId = 'input';
            var isFirstDown = true;
            CodeMirror.on(inp, "keydown", function (e) {
                console.log(e.keyCode);
                // if(e.keyCode == 8 || e.keyCode == 46){
                //     //delete
                //     return;
                // }
                if(e.keyCode == 27){
                    //esc
                    setTimeout(function () { cm.focus(); }, 20);
                    setTimeout(function () { widget.close(); }, 20);
                    CodeMirror.e_stop(e);
                    return;
                }
                if(e.keyCode == 40){
                    //down
                    var i = widget.selectedHint + 1;
                    widget.changeActive(i,{isFunc:true,flag :'down'});
                    CodeMirror.e_stop(e);
                    return;
                }
                if(e.keyCode == 38){
                    //up
                    widget.changeActive(widget.selectedHint - 1,{isFunc:isFirstDown,flag :'up'});
                    isFirstDown = false;
                    CodeMirror.e_stop(e);
                    return;
                }

                if (e.keyCode == 13) {
                    try{
                        var arr = $($('.CodeMirror-hint-active')[0]).attr('class').split('_');
                        if(arr.length == 3){
                            cm.setCursor(Number(arr[1]), Number(arr[2].split(' ')[0]));
                            setTimeout(function () { cm.focus(); }, 20);
                            setTimeout(function () { widget.close(); }, 20);
                            return;
                        }
                    }catch (ee){

                    }

                    return;
                }

                setTimeout(function () {
                    var nodes = hints.childNodes;
                    if(nodes.length<=1)return;
                    for (var i = 1;i<nodes.length;i++){
                        var node = nodes[i];
                        if(inp.value.length == 0){
                            node.className =node.className.replace(" CodeMirror-hint-hide", "");
                        }else if(node.innerHTML.split(inp.value).length==1){
                            if(node.className.indexOf(' CodeMirror-hint-hide')==-1){
                                node.className += ' CodeMirror-hint-hide';
                            }
                        }else{
                            node.className =node.className.replace(" CodeMirror-hint-hide", "");
                        }
                        node.className =node.className.replace(" CodeMirror-hint-active", "");
                    }

                    widget.changeActive(2,{isFunc:true,flag :'down',isClear:true});
                },20);

            });
        }

        if(data.isQuick){

        }
        var completions = data.list;
        for (var i = 0; i < completions.length; ++i) {
            var elt = hints.appendChild(document.createElement("li")), cur = completions[i];
            var className = HINT_ELEMENT_CLASS + (i != this.selectedHint ? "" : " " + ACTIVE_HINT_ELEMENT_CLASS);
            if (cur.className != null) className = cur.className + " " + className;
            if(cur.range){
                className += ' pos_' + cur.range.end.line + "_" +cur.range.end.ch;
            }
            elt.className = className;
            if (cur.render) cur.render(elt, data, cur);
            else elt.appendChild(document.createTextNode(cur.displayText || getText(cur)));
            elt.hintId = i;
        }

        var pos = cm.cursorCoords(completion.options.alignWithWord ? data.from : null);
        var left = pos.left, top = pos.bottom, below = true;
        hints.style.left = left + "px";
        hints.style.top = top + "px";
        // If we're at the edge of the screen, then we want the menu to appear on the left of the cursor.
        var winW = window.innerWidth || Math.max(document.body.offsetWidth, document.documentElement.offsetWidth);
        var winH = window.innerHeight || Math.max(document.body.offsetHeight, document.documentElement.offsetHeight);
        (completion.options.container || document.body).appendChild(hints);
        var box = hints.getBoundingClientRect(), overlapY = box.bottom - winH;
        if (overlapY > 0) {
            var height = box.bottom - box.top, curTop = pos.top - (pos.bottom - box.top);
            if (curTop - height > 0) { // Fits above cursor
                hints.style.top = (top = pos.top - height) + "px";
                below = false;
            } else if (height > winH) {
                hints.style.height = (winH - 5) + "px";
                hints.style.top = (top = pos.bottom - box.top) + "px";
                var cursor = cm.getCursor();
                if (data.from.ch != cursor.ch) {
                    pos = cm.cursorCoords(cursor);
                    hints.style.left = (left = pos.left) + "px";
                    box = hints.getBoundingClientRect();
                }
            }
        }
        var overlapX = box.right - winW;
        if (overlapX > 0) {
            if (box.right - box.left > winW) {
                hints.style.width = (winW - 5) + "px";
                overlapX -= (box.right - box.left) - winW;
            }
            hints.style.left = (left = pos.left - overlapX) + "px";
        }

        cm.addKeyMap(this.keyMap = buildKeyMap(completion, {
            moveFocus: function (n, avoidWrap) { widget.changeActive(widget.selectedHint + n, avoidWrap); },
            setFocus: function (n) { widget.changeActive(n); },
            menuSize: function () { return widget.screenAmount(); },
            length: completions.length,
            close: function () { completion.close(); },
            pick: function () { widget.pick(); },
            data: data
        }));

        if (completion.options.closeOnUnfocus && !data.isFunc) {
            var closingOnBlur;
            cm.on("blur", this.onBlur = function () { closingOnBlur = setTimeout(function () { completion.close(); }, 100); });
            cm.on("focus", this.onFocus = function () { clearTimeout(closingOnBlur); });
        }

        var startScroll = cm.getScrollInfo();
        cm.on("scroll", this.onScroll = function () {
            var curScroll = cm.getScrollInfo(), editor = cm.getWrapperElement().getBoundingClientRect();
            var newTop = top + startScroll.top - curScroll.top;
            var point = newTop - (window.pageYOffset || (document.documentElement || document.body).scrollTop);
            if (!below) point += hints.offsetHeight;
            if (point <= editor.top || point >= editor.bottom) return completion.close();
            hints.style.top = newTop + "px";
            hints.style.left = (left + startScroll.left - curScroll.left) + "px";
        });

        CodeMirror.on(hints, "dblclick", function (e) {
            var ele = e.target || e.srcElement;
            if(ele &&  ele.nodeName && ele.nodeName== 'INPUT'){
                return;
            }
            var t = getHintElement(hints, ele);
            if (t && t.hintId != null) { widget.changeActive(t.hintId); widget.pick(); }
        });

        CodeMirror.on(hints, "click", function (e) {
            var ele = e.target || e.srcElement;
            if(ele &&  ele.nodeName && ele.nodeName== 'INPUT'){
                CodeMirror.e_stopPropagation(e);
                return;
            }
            var t = getHintElement(hints, ele);
            if (t && t.hintId != null) {
                widget.changeActive(t.hintId);
                widget.pick();// if (completion.options.completeOnSingleClick)
            }
        });

        CodeMirror.on(hints, "mousedown", function (e) {
            var ele = e.target || e.srcElement;
            if(ele &&  ele.nodeName && ele.nodeName== 'INPUT'){
                CodeMirror.e_stopPropagation(e);
                return;
            }

            setTimeout(function () { cm.focus(); }, 20);
        });

        CodeMirror.signal(data, "select", completions[0], hints.firstChild);

        if(inp){
            inp.focus();
        }

        return true;
    }

    Widget.prototype = {
        close: function () {
            if (this.completion.widget != this) return;
            this.completion.widget = null;
            this.hints.parentNode.removeChild(this.hints);
            this.completion.cm.removeKeyMap(this.keyMap);

            var cm = this.completion.cm;
            if (this.completion.options.closeOnUnfocus) {
                cm.off("blur", this.onBlur);
                cm.off("focus", this.onFocus);
            }
            cm.off("scroll", this.onScroll);
        },

        pick: function () {
            this.completion.pick(this.data, this.selectedHint);
        },

        changeActive: function (i, avoidWrap) {
            if (i >= this.data.list.length)
            {
                if(avoidWrap&& avoidWrap.isFunc){
                    if(i > this.data.list.length){
                        i = i -1;
                    }
                }else{
                    i = avoidWrap ? this.data.list.length - 1 : 0;
                }

            }else if(i==1&&avoidWrap&& avoidWrap.isFunc && avoidWrap.flag =='down'){

                i = 2;
            }
            else if (i < 0)
            {
                if(avoidWrap&& avoidWrap.isFunc){
                    if(i==-1){
                        i=1;
                    }
                    else if(i <= 1){
                        i = 2;
                    }
                }else{
                    i = avoidWrap ? 0 : this.data.list.length - 1;
                }

            }
            if (this.selectedHint == i) {
                if(avoidWrap&& avoidWrap.isClear){
                    this.selectedHint = i = i -1;
                }else{
                    return;
                }
            }else if(this.selectedHint<=1&&avoidWrap&& avoidWrap.isClear){
                i=1;
            }
            var childNodes = [];
            for(var y = 0;y<this.hints.childNodes.length;y++){
                if(this.hints.childNodes[y].className.indexOf('hide')>=0){
                    continue;
                }
                childNodes.push(this.hints.childNodes[y]);
            }

            if(i>=childNodes.length){
                this.selectedHint=i=childNodes.length-1;
            }

            var node = childNodes[this.selectedHint];
            if(!node)return;
            if(node.nodeName == 'INPUT'){
                node = this.hints.childNodes[this.selectedHint+1];
                if(!node)return;
            }
            node.className = node.className.replace(" " + ACTIVE_HINT_ELEMENT_CLASS, "");
            node = childNodes[this.selectedHint = i];
            if(!node)return;
            if(node.nodeName == 'INPUT'){
                this.selectedHint = i + 1;
                node = childNodes[this.selectedHint];
                if(!node)return;
            }
            node.className += " " + ACTIVE_HINT_ELEMENT_CLASS;
            if (node.offsetTop < this.hints.scrollTop)
                this.hints.scrollTop = node.offsetTop - 3;
            else if (node.offsetTop + node.offsetHeight > this.hints.scrollTop + this.hints.clientHeight)
                this.hints.scrollTop = node.offsetTop + node.offsetHeight - this.hints.clientHeight + 3;
            CodeMirror.signal(this.data, "select", this.data.list[this.selectedHint], node);
        },

        screenAmount: function () {
            return Math.floor(this.hints.clientHeight / this.hints.firstChild.offsetHeight) || 1;
        }
    };

    CodeMirror.registerHelper("hint", "auto", function (cm, options) {
        return;
        var helpers = cm.getHelpers(cm.getCursor(), "hint"), words;
        if (helpers.length) {
            for (var i = 0; i < helpers.length; i++) {
                var cur = helpers[i](cm, options);
                if (cur && cur.list.length) return cur;
            }
        } else if (words = cm.getHelper(cm.getCursor(), "hintWords")) {
            if (words) return CodeMirror.hint.fromList(cm, { words: words });
        } else if (CodeMirror.hint.anyword) {
            return CodeMirror.hint.anyword(cm, options);
        }
    });

    CodeMirror.registerHelper("hint", "fromList", function (cm, options) {
        var cur = cm.getCursor(), token = cm.getTokenAt(cur);
        var found = [];
        for (var i = 0; i < options.words.length; i++) {
            var word = options.words[i];
            if (word.slice(0, token.string.length) == token.string)
                found.push(word);
        }

        if (found.length) return {
            list: found,
            from: CodeMirror.Pos(cur.line, token.start),
            to: CodeMirror.Pos(cur.line, token.end)
        };
    });

    CodeMirror.commands.autocomplete = CodeMirror.showHint;

    var defaultOptions = {
        hint: CodeMirror.hint.auto,
        completeSingle: true,
        alignWithWord: true,
        closeCharacters: /[\s()\[\]{};:>,]/,
        closeOnUnfocus: true,
        completeOnSingleClick: false,
        container: null,
        customKeys: null,
        extraKeys: null
    };

    CodeMirror.defineOption("hintOptions", null);
});
