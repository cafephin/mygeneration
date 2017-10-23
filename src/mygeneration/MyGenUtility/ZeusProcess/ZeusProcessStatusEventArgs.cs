using System;

namespace MyGeneration
{
    public class ZeusProcessStatusEventArgs : EventArgs
    {
        private bool _isRunning;
        private string _message;
        private Guid _id;
        private Exception _Exception;

        public ZeusProcessStatusEventArgs(Guid id, bool isRunning, string message) : this(id, isRunning, message, null) {}
        public ZeusProcessStatusEventArgs(Guid id, bool isRunning, string message, Exception exception) {
            this._id = id;
            this._isRunning = isRunning;
            this._message = message;
            this._Exception = exception;
        }

        public Guid ID
        {
            get { return _id; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public string Message
        {
            get { return _message; }
        }
    }
}