// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Vipr.Core;
    using Vipr.Core.CodeModel;

    public class OdcmModelProcessingBehavior
    {
        private OdcmModel model { get; }

        private string cmdletPrefix { get; }

        public OdcmModelProcessingBehavior(OdcmModel model, string cmdletPrefix)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.cmdletPrefix = cmdletPrefix ?? throw new ArgumentNullException(nameof(cmdletPrefix));
        }

        /// <summary>
        /// Converts an ODCM model into a list of resources which contain PowerShell cmdlet definitions.
        /// </summary>
        /// <param name="model">The model to convert</param>
        /// <returns>The list of resources</returns>
        public IEnumerable<Resource> ConvertToResources()
        {
            OdcmNode odcmTree = this.model.ConvertToOdcmTree();

            IEnumerable<Resource> resources = ConvertOdcmTreeToResources(odcmTree);

            return resources;
        }

        private IEnumerable<Resource> ConvertOdcmTreeToResources(OdcmNode rootNode)
        {
            throw new NotImplementedException();
        }

        //private IEnumerable<Resource> ProcessClass(OdcmClass @class, Resource parentResource = null)
        //{
        //    if (@class == null)
        //    {
        //        throw new ArgumentNullException(nameof(@class));
        //    }

        //    List<Resource> result = new List<Resource>();

        //    // Create the object representing the resource
        //    string filePath = parentResource != null ? $"{parentResource.OutputFilePath}/{@class.Name}" : "/";
        //    string url = parentResource != null ? $"{parentResource.Url}/{@class.Name}" : "/";
        //    Resource resource = new Resource(filePath, url);

        //    // Add cmdlets
        //    IEnumerable<Cmdlet> cmdlets = this.GetCmdlets(resource, @class);

        //    // Evaluate properties
        //    IEnumerable<OdcmProperty> properties = @class.Properties;
        //    foreach (OdcmProperty property in properties)
        //    {
        //        result.AddRange(ProcessProperty(property, resource));
        //    }
            
        //    return result;
        //}

        //private IEnumerable<Resource> ProcessProperty(OdcmProperty property, Resource parentResource)
        //{
        //    if (property == null)
        //    {
        //        throw new ArgumentNullException(nameof(property));
        //    }

        //    // Get property info
        //    string propertyName = property.Name;
        //    OdcmType propertyType = property.Type;

        //    // Check if the property has it's own URL (i.e. if it's a complex type, collection or another entity)
        //    if () ;

        //    // Get the current file system path and call URL
        //    string currentFilePath;
        //    string currentResourceUrl;
        //    if (parentResource == null)
        //    {
        //        currentFilePath = "/";
        //        currentResourceUrl = "/";
        //    }
        //    else
        //    {
        //        currentFilePath = parentResource.OutputFilePath;
        //        currentResourceUrl = parentResource.Url;
        //    }

        //    // Create a new resource for this property
        //    Resource resource = new Resource()
        //    {
        //        FileSystemPath = parentResource.OutputFilePath + propertyName + "/",
        //        Url = parentResource.Url + propertyName + "/",
        //    };

        //    // Add GET cmdlet
        //    CmdletName cmdletName = new CmdletName("Get", this.cmdletPrefix + propertyName);
        //    Cmdlet getCmdlet = new Cmdlet(cmdletName)
        //    {
        //        HttpMethod = HttpMethod.Get,
        //        CallUrl = resource.Url
        //    };

        //    getCmdlet.Parameters.DefaultParameterSet.Add(new Parameter(property.Name, typeof(string))
        //    {
        //        IsMandatory = property.IsRequired
        //    });

        //    yield return resource;
        //}

        //private IEnumerable<Cmdlet> GetCmdlets(Resource resource, OdcmObject obj)
        //{
        //    // Get/Search
        //    Cmdlet getCmdlet = new Cmdlet(new CmdletName("Get", obj.Name))
        //    {
        //        HttpMethod = HttpMethod.Get,
        //        CallUrl = resource.Url + obj.Name + "/",
        //    };
        //    getCmdlet.Parameters.DefaultParameterSet.Add(new Parameter(obj.));
        //    yield return getCmdlet;
            
        //    // Post
        //    if (obj.Projection.SupportsInsert())
        //    {
        //        Cmdlet postCmdlet = null;
        //        yield return postCmdlet;
        //    }

        //    // Patch
        //    if (obj.Projection.SupportsUpdate())
        //    {
        //        Cmdlet patchCmdlet = null;
        //        yield return patchCmdlet;
        //    }

        //    // Delete
        //    if (obj.Projection.SupportsDelete())
        //    {
        //        Cmdlet deleteCmdlet = null;
        //        yield return deleteCmdlet;
        //    }
        //}
    }
}
