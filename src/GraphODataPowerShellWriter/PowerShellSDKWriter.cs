// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace GraphODataPowerShellTemplateWriter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using PowerShellGraphSDK;
    using Vipr.Core;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// A writer module for Vipr which generates C# code that can be compiled into a PowerShell Graph SDK module.
    /// </summary>
    public class PowerShellSDKWriter : IOdcmWriter
    {
        public const string CmdletPrefix = "Graph";

        /// <summary>
        /// Implementation which is provided to Vipr for transforming an ODCM model into the PowerShell SDK.
        /// </summary>
        /// <param name="model">The ODCM model provided by Vipr</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public IEnumerable<TextFile> GenerateProxy(OdcmModel model)
        {
            //IEnumerable<TextFile> generated =  GenerateTestOutput_Simple(model);
            //IEnumerable<TextFile> generated = GenerateTestOutput_Routes(model);
            //IEnumerable<TextFile> generated = GenerateTestOutput_Resources(model);

            IEnumerable<TextFile> generated =  GeneratePowerShellSDK(model);

            return generated;
        }

        /// <summary>
        /// Generates the PowerShell SDK from the given ODCM model.
        /// </summary>
        /// <param name="model">The ODCM model</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public static IEnumerable<TextFile> GeneratePowerShellSDK(OdcmModel model)
        {
            // Convert the ODCM model into nodes (i.e. routes)
            foreach (OdcmNode node in model.ConvertToOdcmNodes())
            {
                // Convert the route into an abstract representation of the PowerShell cmdlets
                Resource resource = node.ConvertToResource(@"PowerShellCmdlets\Generated");

                // Convert the resource into an abstract representation of the C# file
                CSharpFile cSharpFile = resource.ToCSharpFile();

                // Generate the text file by inserting data from the intermediate type into templates
                TextFile outputFile = cSharpFile.ToTextFile();

                // Return the generated file
                yield return outputFile;
            }
        }

        private static IEnumerable<TextFile> GenerateTestOutput_Resources(OdcmModel model)
        {
            IEnumerable<Resource> resources = model.ConvertToOdcmNodes().Select(node => node.ConvertToResource());
            foreach (Resource resource in resources)
            {
                StringBuilder output = new StringBuilder();
                int indentLevel = 0;
                foreach (Cmdlet cmdlet in resource.Cmdlets)
                {
                    // Cmdlet name
                    output.AppendLine($"\"{cmdlet.Name}\" : {{".Indent(indentLevel));
                    indentLevel++;

                    // Cmdlet info
                    output.AppendLine($"\"baseType\" : \"{cmdlet.OperationType.ToCSharpString()}\",".Indent(indentLevel));
                    output.AppendLine($"\"url\" : \"{cmdlet.CallUrl}\",".Indent(indentLevel));

                    // Print parameter sets
                    output.AppendLine("\"parameterSets\" : {".Indent(indentLevel));
                    indentLevel++;
                    foreach (CmdletParameterSet parameterSet in cmdlet.ParameterSets)
                    {
                        // Parameter set name
                        output.AppendLine($"\"{parameterSet.Name}\" : {{".Indent(indentLevel));

                        // Print parameters
                        indentLevel++;
                        CmdletParameter lastParameter = parameterSet.LastOrDefault();
                        foreach (CmdletParameter parameter in parameterSet)
                        {
                            string endOfLine = parameter == lastParameter ? string.Empty : ",";
                            output.AppendLine($"\"{parameter.Name}\" : \"{parameter.Type.ToString()}\"{endOfLine}".Indent(indentLevel));
                        }
                        indentLevel--;
                        output.AppendLine("}".Indent(indentLevel));
                    }
                    indentLevel--;
                    output.AppendLine("}".Indent(indentLevel));

                    indentLevel--;
                    output.AppendLine("}".Indent(indentLevel));
                }

                yield return new TextFile(resource.RelativeFilePath + ".txt", output.ToString());
            }
        }

        private static IEnumerable<TextFile> GenerateTestOutput_Routes(OdcmModel model)
        {
            StringBuilder output = new StringBuilder();
            int maxRoutesPerFile = 1000; // max number of routes in a single output file (decrease this to reduce memory usage)
            int routeCount = 0;
            IEnumerable<OdcmNode> nodes = model.ConvertToOdcmNodes();
            foreach (OdcmNode node in nodes)
            {
                // Every {maxRoutesPerFile} routes, write the output to a file
                if (routeCount != 0 && routeCount % maxRoutesPerFile == 0)
                {
                    int fileNum = routeCount / maxRoutesPerFile;
                    yield return new TextFile($"output_{fileNum}.txt", output.ToString());

                    output = new StringBuilder();
                }

                // Create an OData route from the node
                ODataRoute route = new ODataRoute(node);

                // Write the OData route to the output string
                output.AppendLine(route.ToString());

                // Get the ODCM property from the node
                OdcmProperty currentProperty = node.OdcmProperty;

                // Write the type and it's properties in JSON format
                IEnumerable<OdcmProperty> properties = currentProperty.Type.EvaluateProperties();
                OdcmClass currentClass = currentProperty.Type as OdcmClass;
                output.AppendLine("{");
                foreach (OdcmProperty prop in properties.OrderBy(p => p.CanonicalName()))
                {
                    string inheritedClassName = prop.Class == currentClass ? string.Empty : $" (inherited from '{prop.Class.CanonicalName()}')";
                    output.AppendLine($"\"{prop.CanonicalName()}\" : \"{prop.Type.CanonicalName()}{inheritedClassName}\",".Indent());
                }
                output.AppendLine($"\"{ODataConstants.ObjectProperties.Type}\" : \"{node.OdcmProperty.Type.CanonicalName()}\"".Indent());
                output.AppendLine("}");

                // Increment total count of found routes
                routeCount++;
            }

            // Write the remaining lines to a file
            if (output.Length > 0)
            {
                int fileNum = routeCount / maxRoutesPerFile;
                yield return new TextFile($"output_{fileNum}.txt", output.ToString());
            }
        }

        private static IEnumerable<TextFile> GenerateTestOutput_Simple(OdcmModel model)
        {
            string output = string.Empty;
            int indentLevel = 0;

            output += "Entity Container: " + model.EntityContainer.FullName + " - " + model.EntityContainer.Namespace.Name;

            indentLevel++;
            foreach (OdcmProperty property in model.EntityContainer.Properties)
            {
                output += "\n" + property.Name.Indent(indentLevel);
            }
            indentLevel--;

            output += "\n\n";

            // Namespaces
            foreach (OdcmNamespace @namespace in model.Namespaces)
            {
                output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "== " + @namespace.Name + " ==";

                // Types
                if (@namespace.Types.Any())
                {
                    output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Types:";
                    indentLevel++;
                    foreach (OdcmType type in @namespace.Types)
                    {
                        output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + " - " + type.FullName;
                    }
                    indentLevel--;
                }

                // Type definitions
                if (@namespace.TypeDefinitions.Any())
                {
                    output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Type Definitions:";
                    indentLevel++;
                    foreach (OdcmTypeDefinition typeDefinition in @namespace.TypeDefinitions)
                    {
                        output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + " - " + typeDefinition.FullName + " : " + typeDefinition.BaseType.FullName;
                    }
                    indentLevel--;
                }

                // Enums
                if (@namespace.Enums.Any())
                {
                    output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Enums:";
                    indentLevel++;
                    foreach (OdcmEnum @enum in @namespace.Enums)
                    {
                        output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "= " + @enum.FullName + " =";
                        indentLevel++;
                        foreach (OdcmEnumMember enumMember in @enum.Members)
                        {
                            output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + " - " + enumMember.Name;
                        }
                        indentLevel--;
                    }
                    indentLevel--;
                }

                // Classes
                if (@namespace.Classes.Any())
                {
                    output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Classes:";
                    indentLevel++;
                    foreach (OdcmClass @class in @namespace.Classes)
                    {
                        output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "= " + @class.Name + " =";

                        // Structural Properties
                        IEnumerable<OdcmProperty> structuralProperties = @class.StructuralProperties();
                        if (@class.StructuralProperties().Any())
                        {
                            output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Properties:";
                            indentLevel++;
                            foreach (OdcmProperty property in structuralProperties.OrderBy(val => val.Name))
                            {
                                string propertyName = property.Name;
                                string propertyType = property.Type.FullName;
                                output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + $"{propertyName}\t{propertyType}";
                            }
                            indentLevel--;
                        }

                        // Navigation Properties
                        IEnumerable<OdcmProperty> navigationProperties = @class.NavigationProperties();
                        if (navigationProperties.Any())
                        {
                            output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Navigation Properties:";
                            indentLevel++;
                            foreach (OdcmProperty property in navigationProperties)
                            {
                                string propertyName = property.Name;
                                string propertyType = property.Type.FullName;
                                output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + $"{propertyName}\t{propertyType}";
                            }
                            indentLevel--;
                        }

                        // Methods
                        if (@class.Methods.Any())
                        {
                            output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + "Methods:";
                            indentLevel++;
                            foreach (OdcmMethod method in @class.Methods)
                            {
                                string returnType = "void";
                                if (method.ReturnType != null)
                                {
                                    returnType = method.ReturnType.FullName;
                                }
                                string methodName = method.Name;
                                string parameters = string.Join(", ", method.Parameters.Select(param => param.Type.FullName));
                                string methodDescription = method.LongDescription;
                                output += "\n" + StringUtils.GetIndentPrefix(indentLevel) + $"{returnType} {methodName}({parameters})";
                                if (!string.IsNullOrWhiteSpace(methodDescription))
                                {
                                    output += "\n" + StringUtils.GetIndentPrefix(indentLevel + 1) + methodDescription;
                                }
                            }
                            indentLevel--;
                        }
                    }
                    indentLevel--;
                }

                output += "\n";
            }

            Console.WriteLine(output);
            yield return new TextFile("test.txt", output);
        }
    }
}
