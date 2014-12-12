
function TestType() {}

TestType.prototype.getText = function() { return "testText"; };

testRequire.exports = new TestType();