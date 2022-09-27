// ------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.
// ------------------------------------------------------------------------------

namespace Microsoft.Graph
{
    using Microsoft.Kiota.Abstractions;
    using Microsoft.Kiota.Abstractions.Serialization;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Graph service exception.
    /// </summary>
    public class ServiceException : ApiException,IParsable, IAdditionalDataHolder
    {
        /// <summary>
        /// Creates a new service exception.
        /// </summary>
        /// <param name="error">The error that triggered the exception.</param>
        /// <param name="innerException">The possible innerException.</param>
        public ServiceException(Error error, Exception innerException = null)
            : this(error, responseHeaders: null, statusCode: default(System.Net.HttpStatusCode), innerException: innerException)
        {
        }

        /// <summary>
        /// Creates a new service exception.
        /// </summary>
        /// <param name="error">The error that triggered the exception.</param>
        /// <param name="innerException">The possible innerException.</param>
        /// <param name="responseHeaders">The HTTP response headers from the response.</param>
        /// <param name="statusCode">The HTTP status code from the response.</param>
        public ServiceException(Error error, System.Net.Http.Headers.HttpResponseHeaders responseHeaders, System.Net.HttpStatusCode statusCode, Exception innerException = null)
            : base(error?.ToString(), innerException)
        {
            this.Error = error;
            this.ResponseHeaders = responseHeaders;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Creates a new service exception.
        /// </summary>
        /// <param name="error">The error that triggered the exception.</param>
        /// <param name="innerException">The possible innerException.</param>
        /// <param name="responseHeaders">The HTTP response headers from the response.</param>
        /// <param name="statusCode">The HTTP status code from the response.</param>
        /// <param name="rawResponseBody">The raw JSON response body.</param>
        public ServiceException(Error error, 
                                System.Net.Http.Headers.HttpResponseHeaders responseHeaders,
                                System.Net.HttpStatusCode statusCode, 
                                string rawResponseBody,
                                Exception innerException = null)
            : this(error, responseHeaders, statusCode, innerException)
        {
            this.RawResponseBody = rawResponseBody;
        }

        /// <summary>
        /// The error from the service exception.
        /// </summary>
        public Error Error { get; private set; }

        // ResponseHeaders and StatusCode exposed as pass-through.

        /// <summary>
        /// The HTTP response headers from the response.
        /// </summary>
        public System.Net.Http.Headers.HttpResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// The HTTP status code from the response.
        /// </summary>
        public System.Net.HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Provide the raw JSON response body.
        /// </summary>
        public string RawResponseBody { get; private set; }

        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Checks if a given error code has been returned in the response at any level in the error stack.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>True if the error code is in the stack.</returns>
        public bool IsMatch(string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode))
            {
                throw new ArgumentException("errorCode cannot be null or empty", nameof(errorCode));
            }

            var currentError = this.Error;

            while (currentError != null)
            {
                if (string.Equals(currentError.Code, errorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                currentError = currentError.InnerError;
            }

            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $@"Status Code: {this.StatusCode}{Environment.NewLine}{base.ToString()}";
        }

        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>> {
                {"error", n => { Error = n.GetObjectValue<Error>(Error.CreateFromDiscriminatorValue); } },
                {"statusCode", n => { StatusCode = n.GetEnumValue<System.Net.HttpStatusCode>().Value; } },
                {"rawResponseBody", n => { RawResponseBody = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        /// </summary>
        public void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<Error>("error", Error);
            writer.WriteEnumValue<System.Net.HttpStatusCode>("statusCode", StatusCode);
            writer.WriteStringValue("rawResponseBody", RawResponseBody);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}