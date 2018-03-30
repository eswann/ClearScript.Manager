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
                    "template": "for (var ${element} in ${iterable}) {\n  ${cursor}\n}"
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
                    "name": "require",
                    "description": "require js or dll",
                    "template": "require(${name}); \n${cursor}"
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
                    "name": "function",
                    "description": "anonymous function with param",
                    "template": "function (${condition}) {\n  ${line_selection}${cursor}\n}"
                },
                {
                    "name": "function",
                    "description": "function with param",
                    "template": "function ${name}(${condition}) {\n  ${cursor}\n}"
                },
                {
                    "name": "new",
                    "description": "create new object",
                    "template": "var ${name} = new ${type}();\n${cursor}"
                },
                {
                    "name": "lazy",
                    "description": "lazy creation",
                    "template": "if (${name} == null) {\n  ${name} = new ${type}(${arguments});\n  ${cursor}\n}\n\nreturn ${name};"
                },
                {
                    "name": "while",
                    "description": "while loop with condition",
                    "template": "while (${condition}) {\n  ${line_selection}${cursor}\n}"
                },
                {
                    "name": "null",
                    "description": "null",
                    "template": "null"
                },
                {
                    "name": "true",
                    "description": "true",
                    "template": "true"
                },
                {
                    "name": "return",
                    "description": "return",
                    "template": "return"
                },
                {
                    "name": "false",
                    "description": "false",
                    "template": "false"
                },
                {
                    "name": "break",
                    "description": "break",
                    "template": "break"
                },
                {
                    "name": "case",
                    "description": "case",
                    "template": "case"
                },
                {
                    "name": "continue",
                    "description": "continue",
                    "template": "continue"
                },
                {
                    "name": "delete",
                    "description": "delete",
                    "template": "delete"
                },
                {
                    "name": "instanceof ",
                    "description": "instanceof",
                    "template": "instanceof"
                },
                {
                    "name": "this",
                    "description": "this",
                    "template": "this"
                },
                {
                    "name": "throw",
                    "description": "throw",
                    "template": "throw"
                },
                {
                    "name": "typeof",
                    "description": "typeof",
                    "template": "typeof"
                },
                {
                    "name": "var",
                    "description": "var",
                    "template": "var"
                },
                {
                    "name": "with",
                    "description": "with",
                    "template": "with"
                },
                {
                    "name": "void",
                    "description": "void",
                    "template": "void"
                },
                {
                    "name": "debugger",
                    "description": "debugger",
                    "template": "debugger"
                }
            ]
        };CodeMirror.templatesHint.addTemplates(templates);
})();