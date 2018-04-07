// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    internal static class JsonUtils
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        /// <summary>
        /// Serializes an object into a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The JSON string</returns>
        public static string WriteJson(object obj)
        {
            PSObject psObject = obj as PSObject;
            if (psObject != null)
            {
                IDictionary<string, object> dictionary = psObject.Properties.ToDictionary(
                    (prop) => prop.Name,
                    (prop) => prop.Value);
                return JsonConvert.SerializeObject(dictionary, jsonSettings);
            }
            else
            {
                return JsonConvert.SerializeObject(obj, jsonSettings);
            }
        }

        /// <summary>
        /// Deserializes a JSON string into a JToken object.
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>The deserialized JToken object</returns>
        public static JToken ReadJson(string json)
        {
            return JsonConvert.DeserializeObject<JToken>(json, jsonSettings);
        }

        /// <summary>
        /// Deserializes a JSON string into an object of the given type.
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>The deserialized object</returns>
        public static T ReadJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }

        /// <summary>
        /// Converts a JToken into an object that is native to PowerShell.
        /// </summary>
        /// <param name="json">The JToken representing the json</param>
        /// <returns>The native PowerShell object</returns>
        public static PSObject ToPowerShellObject(this JToken json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json), "The provided JToken cannot be null");
            }

            // The token may represent a value or a container (there are no other subtypes in Newtonsoft's Json.NET)
            // Handle values
            JValue value = json as JValue;
            if (value != null)
            {
                return value.Value == null ? null : PSObject.AsPSObject(value.Value);
            }

            // Handle containers
            JContainer container = json as JContainer;
            if (container != null)
            {
                if (container is JConstructor)
                {
                    throw new ArgumentException("The provided JToken cannot contain a JConstructor object", nameof(json));
                }
                if (container is JProperty)
                {
                    throw new ArgumentException("The provided JToken cannot contain a JProperty object which is not nested inside a JObject", nameof(json));
                }

                JObject obj = container as JObject;
                if (obj != null)
                {
                    PSObject hashtable = new PSObject();
                    foreach (JProperty property in obj.Properties())
                    {
                        // Recursively convert this JObject into a PSObject
                        hashtable.Members.Add(new PSNoteProperty(property.Name, property.Value.ToPowerShellObject()));
                    }

                    return PSObject.AsPSObject(hashtable);
                }

                JArray array = container as JArray;
                if (array != null)
                {
                    List<PSObject> objs = new List<PSObject>();
                    foreach (JToken arrayItem in array)
                    {
                        // Recursively convert this JArray into a PSObject array
                        objs.Add(PSObject.AsPSObject(arrayItem.ToPowerShellObject()));
                    }

                    return PSObject.AsPSObject(objs.ToArray());
                }
            }

            // This should not be possible - we should have returned earlier unless there is a new JToken subtype
            throw new ArgumentException("The JToken is not a value or container type", nameof(json));
        }
    }
}
