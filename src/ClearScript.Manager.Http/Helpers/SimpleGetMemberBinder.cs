using System.Dynamic;

namespace ClearScript.Manager.Http.Helpers
{
    public class SimpleGetMemberBinder : GetMemberBinder
    {
        public SimpleGetMemberBinder(string name) : base(name, true)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            return null;
        }
    }
}