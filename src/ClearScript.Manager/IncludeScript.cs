using System.Globalization;

namespace ClearScript.Manager
{
    /// <summary>
    /// Script to include in the execution of another script.  This script is executed first.  Can contain functions, libraries etc...
    /// </summary>
    public class IncludeScript
    {
        /// <summary>
        /// Unique name of the script to execute.
        /// </summary>
        public string ScriptId { get; set; }

        /// <summary>
        /// Uri (file or Url) of the script to execute.  Need to include script code or script Url.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// This code is prepended to the created code.
        /// </summary>
        public string PrependCode { get; set; }

        /// <summary>
        /// This code is appended to the created code.
        /// </summary>
        public string AppendCode { get; set; }

        /// <summary>
        /// Code of the script to include.  Need to include script code or script Url.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Sets the script Id if it is not set.
        /// </summary>
        internal void EnsureScriptId()
        {
            if (string.IsNullOrEmpty(ScriptId))
            {
                if (!string.IsNullOrEmpty(Uri))
                {
                    ScriptId = Uri;
                }
                else if (!string.IsNullOrEmpty(Code))
                {
                    ScriptId = Code.GetHashCode().ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}