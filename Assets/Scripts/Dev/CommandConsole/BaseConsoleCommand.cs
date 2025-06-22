namespace DevTools
{
    using System;
    using UnityEngine;

    public class BaseConsoleCommand 
    {
        public string CommandID { get; protected set; } 
        public string Description { get; protected set; }
        public string Format { get; protected set; }

        public BaseConsoleCommand(string commandID, string description, string format)
        {
            CommandID = commandID;
            Description = description;
            Format = format;
        }
    }

    public class ConsoleCommand : BaseConsoleCommand
    {
        private Action commandCallback;

        public ConsoleCommand(string commandID, string description, string format, Action callback) : base(commandID, description, format)
        {
            commandCallback = callback;
        }

        public void Invoke()
        {
            if (commandCallback != null) 
                commandCallback();
        }
    }

    public class ConsoleCommand<T1> : BaseConsoleCommand
    {
        private Action<T1> commandCallback;

        public ConsoleCommand(string commandID, string description, string format, Action<T1> commandCallback) : base(commandID, description, format)
        {
            this.commandCallback = commandCallback;
        }

        public void Invoke(T1 param)
        {
            if (commandCallback != null)
                commandCallback(param);
        }
    }
}