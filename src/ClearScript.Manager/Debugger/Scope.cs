namespace JavaScript.Manager.Debugger
{
    using Newtonsoft.Json;

    public class Scope
    {
        /// <summary>
        /// Index of this scope in the scope chain.
        /// </summary>
        /// <remarks>
        /// Index 0 is the top scope and the global scope will always have the highest index for a frame.
        /// </remarks>
        [JsonProperty("index")]
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Index of the Frame
        /// </summary>
        [JsonProperty("frameIndex")]
        public int FrameIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the scope
        /// </summary>
        [JsonProperty("type")]
        public ScopeType Type
        {
            get;
            set;
        }

        /// <summary>
        /// The scope object defining the content of the scope.
        /// </summary>
        /// <remarks>
        /// For local and closure scopes this is transient objects, which has a negative handle value
        /// </remarks>
        [JsonProperty("object")]
        public dynamic Object
        {
            get;
            set;
        }
    }
}
