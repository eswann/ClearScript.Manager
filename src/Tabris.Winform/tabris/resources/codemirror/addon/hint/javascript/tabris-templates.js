(function() {
  var templates = {
    "name": "javascript",
    "context": "javascript",
    "templates": [{
      "name": "properties",
      "description": "With LayoutData containing `left`, `top` and `right` arguments.",
      "template": "{layoutData: {left: 0, top: 0, right: 0}}"
    }, {
      "name": "properties",
      "description": "With Text, Font and LayoutData containing `left`, `top` and `right` arguments.",
      "template": "{text: ${text}, font: \"14px\", layoutData: {left: 0, top: 0, right: 0}}"
    }, {
      "name": "animationProperties",
      "description": "With the `opacity` property set for a fade out effect.",
      "template": "{opacity: 0}"
    }, {
      "name": "animationProperties",
      "description": "With the `translation` property set for a horizontal translation animation.",
      "template": "{transform: {translationX: 20}}"
    }, {
      "name": "options",
      "description": "Animation `option` property with delay and duration.",
      "template": "{delay: 1000, duration: 2000}"
    }, {
      "name": "options",
      "description": "Animation `option` property with duration and easing.",
      "template": "{duration: 2000, easing: \"ease-out\"}"
    }]
  };
  CodeMirror.templatesHint.addTemplates(templates);
})();
