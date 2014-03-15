namespace Stratoflow.ClearScript.Test
{
    public class ScriptSubject
    {
        private bool _isSuccess = true;

        // ReSharper disable once InconsistentNaming
        public object data { get; set; }

        // ReSharper disable once InconsistentNaming
        public bool isSuccess
        {
            get { return _isSuccess; }
            set { _isSuccess = value; }
        }
    }
}