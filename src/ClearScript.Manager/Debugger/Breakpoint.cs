namespace JavaScript.Manager.Debugger
{
    using Newtonsoft.Json;

    public class Breakpoint
    {
        public Breakpoint()
        {
            Condition = string.Empty;
        }

        public Breakpoint(int lineNumber)
            : this()
        {
            this.LineNumber = lineNumber;
        }

        [JsonIgnore]
        public string BreakPointNumber
        {
            get;
            set;
        }

       

        [JsonProperty("url")]
        public string TargetId
        {
            get;
            set;
        }

        [JsonProperty("lineNumber")]
        public int LineNumber
        {
            get;
            set;
        }

        [JsonProperty("columnNumber")]
        public int? Column
        {
            get;
            set;
        }

      

        [JsonProperty("condition")]
        public string Condition
        {
            get;
            set;
        }

    }
}
