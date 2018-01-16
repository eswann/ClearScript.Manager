namespace JavaScript.Manager.Debugger.Messages
{
    public static class ResponseExtensions
    {
        //public static IEnumerable<Breakpoint> GetBreakpoints(this Response response)
        //{
        //    if (response == null)
        //        throw new ArgumentNullException("response");

        //    if (response.Command != "listbreakpoints")
        //        throw new ArgumentException("The specified response object was not a listbreakpoints response.", "response");

        //    if (response.Success == false)
        //        throw new ArgumentException("The specified response object did not specify a success condition.", "response");

        //    foreach (var breakpoint in response.Body.breakpoints)
        //    {
        //        var concreteBreakpoint = new Breakpoint
        //        {
        //            BreakPointNumber = breakpoint.number,
        //            ScriptId = breakpoint.script_id,
        //            ScriptName = breakpoint.script_name,
        //            LineNumber = breakpoint.line,
        //            Column = breakpoint.column,
        //            GroupId = breakpoint.groupId,
        //            HitCount = breakpoint.hit_count,
        //            Enabled = breakpoint.active,
        //            IgnoreCount = breakpoint.ignoreCount
        //            //ActualLocations
        //        };
        //        yield return concreteBreakpoint;
        //    }
        //}

        //public static IEnumerable<Scope> GetScopes(this Response response)
        //{
        //    if (response == null)
        //        throw new ArgumentNullException("response");

        //    if (response.Command != "scopes")
        //        throw new ArgumentException("The specified response object was not a scopes response.", "response");

        //    if (response.Success == false)
        //        throw new ArgumentException("The specified response object did not specify a success condition.", "response");

        //    foreach (var scope in response.Body.scopes)
        //    {
        //        yield return GetScopeFromDynamic(scope);
        //    }
        //}

        //public static Scope GetScope(this Response response)
        //{
        //    if (response == null)
        //        throw new ArgumentNullException("response");

        //    if (response.Command != "scope")
        //        throw new ArgumentException("The specified response object was not a scopes response.", "response");

        //    if (response.Success == false)
        //        throw new ArgumentException("The specified response object did not specify a success condition.", "response");

        //    return GetScopeFromDynamic(response.Body);
        //}

        //private static Scope GetScopeFromDynamic(dynamic scope)
        //{
        //    var result = new Scope
        //    {
        //        Index = scope.index,
        //        FrameIndex = scope.frameIndex,
        //        Type = (ScopeType)Enum.ToObject(typeof(ScopeType), (byte)scope.type),
        //        Object = scope["object"]
        //    };

        //    return result;
        //}
    }
}
