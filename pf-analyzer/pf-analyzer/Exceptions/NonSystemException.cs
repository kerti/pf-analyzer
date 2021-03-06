﻿using System;
using System.Runtime.Serialization;

namespace PFAnalyzer.Exceptions
{
    /// <summary>
    /// This class is an abstract class that is meant to be the base of all exceptions that are manually thrown by
    /// user code, which then need to be addressed separately from system exceptions.
    /// </summary>
    [Serializable]
    public abstract class NonSystemException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonSystemException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        protected NonSystemException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSystemException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual
        /// information about the source or destination. 
        /// </param>
        protected NonSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSystemException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner
        /// exception is specified. 
        /// </param>
        protected NonSystemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
