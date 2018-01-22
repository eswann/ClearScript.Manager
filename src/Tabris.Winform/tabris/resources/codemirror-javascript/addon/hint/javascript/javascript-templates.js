(function () {
    var templates = 
        {
            "name": "javascript",
            "context": "javascript",
            "templates": [
                {
                    "name": "for",
                    "description": "iterate over array",
                    "template": "for (var ${index} = 0; ${index} < ${array}.length; ${index}++) {\n  ${line_selection}${cursor}\n}"
                },
                {
                    "name": "for",
                    "description": "iterate over array with temporary variable",
                    "template": "for (var ${index} = 0; ${index} < ${array}.length; ${index}++) {\n  var ${element} = ${array}[${index}];\n  ${cursor}\n}"
                },
                {
                    "name": "forin",
                    "description": "iterate using for .. in",
                    "template": "for (var ${iterable_element} in ${iterable}) {\n  ${cursor}\n}"
                },
                {
                    "name": "do",
                    "description": "do while statement",
                    "template": "do {\n  ${line_selection}${cursor}\n} while (${condition});"
                },
                {
                    "name": "switch",
                    "description": "switch case statement",
                    "template": "switch (${key}) {\n  case ${value}:\n    ${cursor}\n    break;\n  default:\n    break;\n}"
                },
                {
                    "name": "if",
                    "description": "if statement",
                    "template": "if (${condition}) {\n  ${line_selection}${cursor}\n}"
                },
                {
                    "name": "ifelse",
                    "description": "if else statement",
                    "template": "if (${condition}) {\n  ${cursor}\n} else {\n  \n}"
                },
                {
                    "name": "elseif",
                    "description": "else if block",
                    "template": "else if (${condition}) {\n  ${cursor}\n}"
                },
                {
                    "name": "else",
                    "description": "else block",
                    "template": "else {\n  ${cursor}\n}"
                },
                {
                    "name": "try",
                    "description": "try catch block",
                    "template": "try {\n  ${line_selection}${cursor}\n} catch (e) {\n  // ${todo}: handle exception\n}"
                },
                {
                    "name": "catch",
                    "description": "catch block",
                    "template": "catch (e) {\n  ${cursor}// ${todo}: handle exception\n}"
                },
                {
                    "name": "function",
                    "description": "function",
                    "template": "function ${name}() {\n  ${cursor}\n}"
                },
                {
                    "name": "function",
                    "description": "anonymous function",
                    "template": "function () {\n  ${cursor}\n}"
                },
                {
                    "name": "new",
                    "description": "create new object",
                    "template": "var ${name} = new ${type}(${arguments});"
                },
                {
                    "name": "lazy",
                    "description": "lazy creation",
                    "template": "if (${name:var} == null) {\n  ${name} = new ${type}(${arguments});\n  ${cursor}\n}\n\nreturn ${name};"
                },
                {
                    "name": "<code>",
                    "description": "<code></code>",
                    "template": "<code>${word_selection}${}</code>${cursor}"
                },
                {
                    "name": "null",
                    "description": "<code>null</code>",
                    "template": "null"
                },
                {
                    "name": "true",
                    "description": "<code>true</code>",
                    "template": "true"
                },
                {
                    "name": "false",
                    "description": "<code>false</code>",
                    "template": "false"
                },
                {
                    "name": "<pre>",
                    "description": "<pre></pre>",
                    "template": "<pre>${word_selection}${}</pre>${cursor}"
                },
                {
                    "name": "<b>",
                    "description": "<b></b>",
                    "template": "<b>${word_selection}${}</b>${cursor}"
                },
                {
                    "name": "<i>",
                    "description": "<i></i>",
                    "template": "<i>${word_selection}${}</i>${cursor}"
                },
                {
                    "name": "@author",
                    "description": "author name",
                    "template": "@author ${user}"
                },
                {
                    "name": "while",
                    "description": "while loop with condition",
                    "template": "while (${condition}) {\n  ${line_selection}${cursor}\n}"
                }
            ]
        };CodeMirror.templatesHint.addTemplates(templates);
})();