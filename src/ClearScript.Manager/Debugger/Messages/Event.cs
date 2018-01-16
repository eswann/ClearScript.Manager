namespace JavaScript.Manager.Debugger.Messages
{
    using Newtonsoft.Json;

    public class EventResponse
    {
        /// <summary>
        /// Sequence number of the response.
        /// </summary>
        [JsonProperty("seq")]
        public int Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// The type of response.
        /// </summary>
        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// The event type (Exception/Breakpoint/Compile/etc...
        /// </summary>
        [JsonProperty("event")]
        public string Event
        {
            get;
            set;
        }

        [JsonProperty("body")]
        public dynamic Body
        {
            get;
            set;
        }
    }
}
