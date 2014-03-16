using System;
using Microsoft.ClearScript.V8;
using NUnit.Framework;

namespace ClearScript.Manager.Test
{
    [TestFixture]
    public class V8ScriptingTests
    {
        [Test]
        public void Can_Perform_JS_Length_On_A_Passed_String()
        {
            using (var engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers))
            {
                engine.Script.MyString = "Just a test string";
                engine.AddHostType("Console", typeof(Console)); 

                engine.Execute("Console.WriteLine ('Length of MyString = {0}', MyString.length)");
            }
        }

        [Test]
        public void Can_Perform_JS_ToLower_On_A_Passed_String()
        {
            using (var engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers))
            {
                engine.Script.MyString = "Just a test string";
                engine.AddHostType("Console", typeof(Console));

                engine.Execute("Console.WriteLine ('Length of MyString = {0}', MyString.toLowerCase())");
            }
        }

        [Test]
        public void Can_Perform_JS_ToLower_On_An_Object_Property()
        {
            using (var engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers))
            {
                var myObject = new {MyString = "Just a test string"};

                engine.Script.MyString = "Just a test string";
                engine.AddHostType("Console", typeof(Console));
                engine.AddHostObject("myObject", myObject);

                engine.Execute("Console.WriteLine ('Length of MyString = {0}', myObject.MyString.toLowerCase())");
            }
        }

    }
}