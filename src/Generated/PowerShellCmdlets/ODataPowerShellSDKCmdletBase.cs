// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using PowerShellGraphSDK.Common;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json.Linq;

    public abstract class ODataPowerShellSDKCmdletBase : PSCmdlet
    {
        [Parameter]
        public string GraphVersion { get; set; } = "v1.0";

        protected override sealed void ProcessRecord()
        {
            try
            {
                this.Run();
            }
            catch (Exception ex)
            {
                // Unknown error type
                this.WriteError(ex);
            }
        }

        private void Run()
        {
            // Get the environment parameters
            EnvironmentParameters environmentParameters = GraphAuthentication.EnvironmentParameters;

            // Auth
            AuthenticationResult authResult = environmentParameters?.AuthResult;
            if (authResult == null)
            {
                // User has not authenticated
                throw new PSAuthenticationError(
                    new InvalidOperationException("Not authenticated.  Please use the 'Connect-Intune' cmdlet to authenticate."),
                    "NotAuthenticated",
                    ErrorCategory.AuthenticationError,
                    null);
            }
            else if (authResult.ExpiresOn <= DateTimeOffset.Now)
            {
                // Expired token
                throw new PSAuthenticationError(
                    new InvalidOperationException("Authentication has expired.  Please use the 'Connect-Intune' cmdlet to authenticate."),
                    "AuthenticationExpired",
                    ErrorCategory.AuthenticationError,
                    authResult);
            }

            // Build the URL
            string requestUrl;
            string resourcePath = this.GetResourcePath();
            if (resourcePath == null)
            {
                throw new PSGraphSDKException(
                    new ArgumentNullException(nameof(this.GetResourcePath)),
                    "NullResourceUrl",
                    ErrorCategory.InvalidArgument,
                    resourcePath);
            }
            string baseAddress = environmentParameters.ResourceBaseAddress;
            string tempPath = resourcePath.TrimStart('/'); // remove leading slash if it exists so relative URLs get treated as such
            if (Uri.IsWellFormedUriString(tempPath, UriKind.Absolute))
            {
                requestUrl = tempPath;
            }
            else if (Uri.IsWellFormedUriString(tempPath, UriKind.Relative))
            {
                string sanitizedBaseUrl = $"{baseAddress.TrimEnd('/')}/{GraphVersion}";
                requestUrl = $"{sanitizedBaseUrl}/{tempPath}";
            }
            else
            {
                throw new PSGraphSDKException(
                    new ArgumentException("A valid URL could not be constructed - ensure that the provided resource path a valid relative or absolute URL", nameof(this.GetResourcePath)),
                    "InvalidResourceUrl",
                    ErrorCategory.InvalidArgument,
                    resourcePath);
            }

            // Add the query options to the URL
            IDictionary<string, string> queryOptions = this.GetUrlQueryOptions();
            if (queryOptions != null && queryOptions.Any())
            {
                string queryOptionsString;
                if (requestUrl.Contains("?"))
                {
                    // Query options already exist in the URL
                    queryOptionsString = "&";
                }
                else
                {
                    // Query options don't already exist in the URL
                    queryOptionsString = "?";
                }

                // Construct the query options string
                queryOptionsString += string.Join("&", queryOptions.Select((entry) => $"{entry.Key}={entry.Value}"));

                // Append the query options to the URL
                requestUrl += queryOptionsString;
            }

            // Get HTTP method
            HttpMethod httpMethod = this.GetHttpMethod();
            if (httpMethod == null)
            {
                throw new PSGraphSDKException(
                    new ArgumentNullException(nameof(this.GetHttpMethod)),
                    "InvalidResourceUrl",
                    ErrorCategory.InvalidArgument,
                    resourcePath);
            }

            // Get content
            HttpContent content = null;
            object contentObject = this.GetContent();
            if (contentObject != null)
            {
                content = this.WriteContent(contentObject);
            }

            // Make the HTTP request
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, requestUrl);
            string requestContent = null; // need to evaluate this before making the call otherwise the content object will get disposed
            if (content != null)
            {
                requestMessage.Content = content;
                requestContent = requestMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

            // Handle the result
            string responseContent = responseMessage.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            if (responseMessage.IsSuccessStatusCode)
            {
                // Get the result
                object cmdletResult = null;
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    cmdletResult = this.ReadResponse(responseContent);
                }

                // Write the result to the pipeline
                if (cmdletResult != null)
                {
                    this.WriteObject(cmdletResult);
                }
            }
            else
            {
                // Build the error message
                string errorMessage = $"{(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase}";
                if (responseContent != null)
                {
                    errorMessage += Environment.NewLine + responseContent;
                }

                // Convert request message to a PowerShell object
                PSObject powerShellRequestObject = new PSObject();
                powerShellRequestObject.Members.Add(new PSNoteProperty("HttpMethod", requestMessage.Method.Method));
                powerShellRequestObject.Members.Add(new PSNoteProperty("URL", requestMessage.RequestUri.ToString()));
                PSObject powerShellRequestObjectHeaders = new PSObject();
                foreach (var header in requestMessage.Headers)
                {
                    powerShellRequestObjectHeaders.Members.Add(new PSNoteProperty(header.Key, string.Join(",", header.Value)));
                }
                powerShellRequestObject.Members.Add(new PSNoteProperty("Headers", powerShellRequestObjectHeaders));
                if (requestContent != null)
                {
                    powerShellRequestObject.Members.Add(new PSNoteProperty("Content", requestContent));
                }

                // Convert response message to a PowerShell object
                PSObject powerShellResponseObject = new PSObject();
                powerShellResponseObject.Members.Add(new PSNoteProperty("HttpStatusCode", (int)responseMessage.StatusCode));
                powerShellResponseObject.Members.Add(new PSNoteProperty("HttpStatusPhrase", responseMessage.ReasonPhrase));
                PSObject powerShellResponseObjectHeaders = new PSObject();
                foreach (var header in responseMessage.Headers)
                {
                    powerShellResponseObjectHeaders.Members.Add(new PSNoteProperty(header.Key, string.Join(",", header.Value)));
                }
                powerShellResponseObject.Members.Add(new PSNoteProperty("Headers", powerShellResponseObjectHeaders));
                powerShellResponseObject.Members.Add(new PSNoteProperty("Content", responseContent));

                // Create PowerShell error object
                PSObject powerShellErrorObject = new PSObject();
                powerShellErrorObject.Members.Add(new PSNoteProperty("Request", powerShellRequestObject));
                powerShellErrorObject.Members.Add(new PSNoteProperty("Response", powerShellResponseObject));

                throw new PSGraphSDKException(
                    new HttpRequestException(errorMessage),
                    "HttpRequestError",
                    ErrorCategory.ConnectionError,
                    powerShellErrorObject);
            }
        }

        private void WriteError(Exception ex)
        {
            ErrorRecord errorRecord;
            IContainsErrorRecord powerShellError = ex as IContainsErrorRecord;
            if (powerShellError != null)
            {
                errorRecord = powerShellError.ErrorRecord;
            }
            else
            {
                errorRecord = new ErrorRecord(
                    ex,
                    PSGraphSDKException.ErrorPrefix + "UnknownError",
                    ErrorCategory.OperationStopped,
                    null);
            }

            this.WriteError(errorRecord);
        }

        /// <summary>
        /// Returns a mapping of query options to their values.  This method should never return null.
        /// Implementations of this method should first call <code>base.GetUrlQueryOptions()</code> and then
        /// add additional query options to the result.
        /// 
        /// <para>
        /// NOTE: The keys should be the full query option name (e.g. WITH the "$" prefix for "$select", "$expand", etc.).
        /// </para>
        /// </summary>
        /// <returns>The mapping of query options to their values</returns>
        internal virtual IDictionary<string, string> GetUrlQueryOptions()
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns the HTTP method to be used for the network call.  This method should never return null.
        /// 
        /// <para>This method returns <see cref="HttpMethod.Get"/> if it is not overridden.</para>
        /// </summary>
        /// <returns>The HTTP method</returns>
        internal virtual HttpMethod GetHttpMethod()
        {
            return HttpMethod.Get;
        }

        /// <summary>
        /// Returns the path to the resource.  This may be either a relative or absolute URL.  This method should never return null.
        /// </summary>
        /// <returns>The path to the resource</returns>
        internal abstract string GetResourcePath();

        /// <summary>
        /// Returns the content to be sent in the network call.  This method may return null if there is no content to send.
        /// 
        /// <para>This method returns null if it is not overridden.</para>
        /// </summary>
        /// <returns>The request content</returns>
        internal virtual object GetContent()
        {
            return null;
        }

        /// <summary>
        /// Converts the content obtained from the <see cref="GetContent"/> method into an <see cref="HttpContent"/> object.
        /// 
        /// <para>This method defaults to converting the object to a JSON string and then wrapping it in a <see cref="StringContent"/> object.</para>
        /// </summary>
        /// <param name="content">The content to be converted</param>
        /// <returns>The converted HttpContent object</returns>
        internal virtual HttpContent WriteContent(object content)
        {
            string contentString = JsonUtils.WriteJson(content);
            HttpContent result = new StringContent(contentString);
            return result;
        }

        /// <summary>
        /// Converts the body of an HTTP response to a C# object.
        /// 
        /// <para>This method defaults to assuming a JSON response body, and then converting it to a <see cref="PSObject"/> instance.</para>
        /// </summary>
        /// <param name="content">The HTTP response body</param>
        /// <returns>The converted object</returns>
        internal virtual object ReadResponse(string content)
        {

            JToken jsonToken = JsonUtils.ReadJson(content);
            PSObject result = JsonUtils.ToPowerShellObject(jsonToken);
            return result;
        }
    }
}
