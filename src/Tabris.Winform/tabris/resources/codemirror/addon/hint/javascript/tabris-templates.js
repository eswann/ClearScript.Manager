(function () {
    CodeMirror.quickTemplatesHint = [
        {
            className: "CodeMirror-hint-template",
            text: 'try',
            temp: [
                {text: 'try {', idx: 0},
                {text: '$', idx: 2},
                {text: '} catch (e) {',idx: 0},
                {text: '// todo: handle exception', idx: 2},
                {text: '}', idx: 0}
            ]
        },
        {
            className: "CodeMirror-hint-template",
            text: 'tryf',
            temp: [
                {text: 'try {', idx: 0},
                {text: '$', idx: 2},
                {text: '} catch (e) {',idx: 0},
                {text: '// todo: handle exception', idx: 2},
                {text: '} finally {',idx: 0},
                {text: '// todo: handle finally', idx: 2},
                {text: '}', idx: 0}
            ]
        },
        {
            className: "CodeMirror-hint-template",
            text: 'if',
            temp: [
                {text: 'if (condition){', idx: 0},
                {text: '$', idx: 2},
                {text: '}', idx: 0}
            ]
        },
        {
            className: "CodeMirror-hint-template",
            text: 'for',
            temp: [
                {text: 'for (var i = 0; i < array.length; i++) {', idx: 0},
                {text: '$', idx: 2},
                {text: '}', idx: 0}
            ]
        },
        {
            className: "CodeMirror-hint-template",
            text: 'forin',
            temp: [
                {text: 'for (var element in iterable) {', idx: 0},
                {text: '$', idx: 2},
                {text: '}', idx: 0}
            ]
        }
    ]
})();
