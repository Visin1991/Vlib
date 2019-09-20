using System;

namespace V
{
    public class VC_Log : IEquatable<VC_Log>
    {
        public VC_Log(string logType, string input, bool includeStackTrace = false)
        {
            Input = input;
            Type = logType;
            StackTrace = includeStackTrace ? Environment.StackTrace : string.Empty;
        }

        public string Input { get; private set; }
        public string StackTrace { get; private set; }
        public string Type { get; private set; }
        public bool HasStackTrace
        {
            get
            {
                return StackTrace == string.Empty;
            }
        }
        public bool Equals(VC_Log other)
        {
            return other.Type == Type && other.Input == Input;
        }
        public override int GetHashCode()
        {
            return Input.GetHashCode() + Type.GetHashCode();
        }
    }
}
