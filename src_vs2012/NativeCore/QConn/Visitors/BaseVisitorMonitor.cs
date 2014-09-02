﻿using System;
using System.Threading;

namespace BlackBerry.NativeCore.QConn.Visitors
{
    /// <summary>
    /// Base class for all FileService visitors that want to also support status change notifications and waiting.
    /// </summary>
    public abstract class BaseVisitorMonitor : IFileServiceVisitorMonitor, IDisposable
    {
        private AutoResetEvent _event;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseVisitorMonitor()
        {
            _event = new AutoResetEvent(false);
        }

        ~BaseVisitorMonitor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Resets the state of wait-event fired, when visitor completes its task.
        /// </summary>
        protected void ResetWait()
        {
            if (_event == null)
                throw new ObjectDisposedException("BaseVisitorMonitor");

            _event.Reset();
        }

        /// <summary>
        /// Notifies subscribed listeners, that visitor finished its task and arms the wait-event.
        /// </summary>
        protected void NotifyCompleted()
        {
            if (_event == null)
                throw new ObjectDisposedException("BaseVisitorMonitor");

            var handler = Completed;
            if (handler != null)
                handler(this, EventArgs.Empty);

            _event.Set();
        }

        #region IFileServiceVisitorMonitor Implementation

        public event EventHandler Completed;

        public bool Wait()
        {
            if (_event == null)
                throw new ObjectDisposedException("BaseVisitorMonitor");

            return _event.WaitOne();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_event != null)
                {
                    _event.Dispose();
                    _event = null;
                }
            }
        }

        #endregion
    }
}
