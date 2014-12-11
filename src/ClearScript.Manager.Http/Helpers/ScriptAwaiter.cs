using System;
using System.Threading.Tasks;

namespace ClearScript.Manager.Http.Helpers
{
    public class ScriptAwaiter
    {
        public Task T { get; set; }
        public Action Callback { get; set; }

        public ScriptAwaiter()
        {
            T = new Task(new Action(() => { }));
            Callback = CallbackImp;
        }

        void CallbackImp()
        {
            T.Start();
        }

    }
}