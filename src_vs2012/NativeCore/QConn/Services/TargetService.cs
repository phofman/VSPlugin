﻿using System;

namespace BlackBerry.NativeCore.QConn.Services
{
    /// <summary>
    /// Base class for operational service exposed by QConn.
    /// It can be:
    ///  * process manager
    ///  * file manager
    ///  * profiler
    ///  * ...
    /// </summary>
    public abstract class TargetService : IDisposable
    {
        private QConnConnection _connection;

        /// <summary>
        /// Init constructor.
        /// </summary>
        protected TargetService(Version version, QConnConnection connection)
        {
            if (version == null)
                throw new ArgumentNullException("version");
            if (connection == null)
                throw new ArgumentNullException("connection");

            Version = version;
            _connection = connection;
        }

        #region Properties

        /// <summary>
        /// Gets the version of the service.
        /// </summary>
        public Version Version
        {
            get;
            private set;
        }

        public QConnConnection Connection
        {
            get
            {
                if (_connection == null)
                    throw new ObjectDisposedException("TargetService");
                return _connection;
            }
        }

        protected bool IsDisposed
        {
            get { return _connection == null; }
        }

        #endregion

        /// <summary>
        /// Initializes a connection to a service.
        /// </summary>
        public void Open()
        {
            if (_connection == null)
                throw new ObjectDisposedException("TargetService");

            _connection.Open();
        }

        /// <summary>
        /// Closes the connection to a service and releases all resources.
        /// </summary>
        public void Close()
        {
            if (_connection == null)
                throw new ObjectDisposedException("TargetService");

            if (_connection.IsConnected)
            {
                _connection.Close();
            }
        }

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
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        #endregion
    }
}
