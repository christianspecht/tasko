using System;

namespace Tasko.Server
{
    /// <summary>
    /// custom exception, will be thrown by the Task class
    /// </summary>
    [Serializable]
    public class TaskoInvalidInputException : Exception
    {
        public TaskoInvalidInputException() { }
        public TaskoInvalidInputException(string message) : base(message) { }
        public TaskoInvalidInputException(string message, Exception inner) : base(message, inner) { }
        protected TaskoInvalidInputException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}