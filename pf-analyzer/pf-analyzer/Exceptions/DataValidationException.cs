﻿using System;
using System.Runtime.Serialization;

namespace PFAnalyzer.Exceptions
{
    /// <summary>
    /// An exception class that specifically pertains to data validation logic and any errors that may arise and need
    /// to be addressed from such logic.
    /// </summary>
    [Serializable]
    public class DataValidationException : NonSystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        public DataValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual
        /// information about the source or destination. 
        /// </param>
        public DataValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner
        /// exception is specified. 
        /// </param>
        public DataValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
