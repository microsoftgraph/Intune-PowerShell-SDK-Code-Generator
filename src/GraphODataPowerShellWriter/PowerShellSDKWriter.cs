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
    using Vipr.Core;
    using Vipr.Core.CodeModel;

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
            IEnumerable<TextFile> generated = GenerateTestOutput_Routes(model);
            //IEnumerable<TextFile> generated =  GeneratePowerShellSDK(model);

            return generated;
        }

        /// <summary>
        /// Generates the PowerShell SDK from the given ODCM model.
        /// </summary>
        /// <param name="model">The ODCM model</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public static IEnumerable<TextFile> GeneratePowerShellSDK(OdcmModel model)
        {
            // Convert the ODCM model into the list of nodes (i.e. path segments)
            foreach (OdcmNode node in model.ConvertToOdcmNodes())
            {
                // Convert the tree structure into an abstract representation of the PowerShell cmdlets
                Resource resource = node.ConvertToResource();

                // Generate the text file by inserting data from the intermediate type into templates
                TextFile outputFile = resource.ToTextFile();

                // Return the generated file
                yield return outputFile;
            }
        }

        private static IEnumerable<TextFile> GenerateTestOutput_Routes(OdcmModel model)
        {
            StringBuilder output = new StringBuilder();
            int maxLines = 1000; // max number of lines in a single output file (decrease this to reduce memory usage)
            IEnumerator<OdcmNode> enumerator = model.ConvertToOdcmNodes().GetEnumerator();
            int routeNum = 0;
            while (enumerator.MoveNext())
            {
                // Every {maxLines} lines, write the output to a file
                if (routeNum != 0 && routeNum % maxLines == 0)
                {
                    int fileNum = routeNum / maxLines;
                    yield return new TextFile($"output_{fileNum}.txt", output.ToString());

                    output = new StringBuilder();
                }

                // Get the node
                OdcmNode node = enumerator.Current;

                // Write the OData route to the node
                output.AppendLine(node.EvaluateODataRoute());

                // Write the type and it's properties
                IEnumerable<OdcmProperty> properties = node.OdcmProperty.Type.EvaluateProperties();
                // Write the properties in JSON format
                output.AppendLine("{");
                foreach (OdcmProperty prop in properties)
                {
                    output.AppendLine(StringUtils.Indent(1, $"\"{prop.CanonicalName()}\" : \"{prop.Type.CanonicalName()} ({prop.Class.CanonicalName()})\","));
                }
                output.AppendLine(StringUtils.Indent(1, $"\"@odata.type\" : \"{node.OdcmProperty.Type.CanonicalName()}\""));
                output.AppendLine("}");

                // Increment total count of found routes
                routeNum++;
            }

            // Write the remaining lines to a file
            if (output.Length > 0)
            {
                int fileNum = routeNum / maxLines;
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
                output += "\n" + StringUtils.Indent(indentLevel, property.Name);
            }
            indentLevel--;

            output += "\n\n";

            // Namespaces
            foreach (OdcmNamespace @namespace in model.Namespaces)
            {
                output += "\n" + StringUtils.IndentPrefix(indentLevel) + "== " + @namespace.Name + " ==";

                // Types
                if (@namespace.Types.Any())
                {
                    output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Types:";
                    indentLevel++;
                    foreach (OdcmType type in @namespace.Types)
                    {
                        output += "\n" + StringUtils.IndentPrefix(indentLevel) + " - " + type.FullName;
                    }
                    indentLevel--;
                }

                // Type definitions
                if (@namespace.TypeDefinitions.Any())
                {
                    output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Type Definitions:";
                    indentLevel++;
                    foreach (OdcmTypeDefinition typeDefinition in @namespace.TypeDefinitions)
                    {
                        output += "\n" + StringUtils.IndentPrefix(indentLevel) + " - " + typeDefinition.FullName + " : " + typeDefinition.BaseType.FullName;
                    }
                    indentLevel--;
                }

                // Enums
                if (@namespace.Enums.Any())
                {
                    output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Enums:";
                    indentLevel++;
                    foreach (OdcmEnum @enum in @namespace.Enums)
                    {
                        output += "\n" + StringUtils.IndentPrefix(indentLevel) + "= " + @enum.FullName + " =";
                        indentLevel++;
                        foreach (OdcmEnumMember enumMember in @enum.Members)
                        {
                            output += "\n" + StringUtils.IndentPrefix(indentLevel) + " - " + enumMember.Name;
                        }
                        indentLevel--;
                    }
                    indentLevel--;
                }

                // Classes
                if (@namespace.Classes.Any())
                {
                    output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Classes:";
                    indentLevel++;
                    foreach (OdcmClass @class in @namespace.Classes)
                    {
                        output += "\n" + StringUtils.IndentPrefix(indentLevel) + "= " + @class.Name + " =";

                        // Structural Properties
                        IEnumerable<OdcmProperty> structuralProperties = @class.StructuralProperties();
                        if (@class.StructuralProperties().Any())
                        {
                            output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Properties:";
                            indentLevel++;
                            foreach (OdcmProperty property in structuralProperties.OrderBy(val => val.Name))
                            {
                                string propertyName = property.Name;
                                string propertyType = property.Type.FullName;
                                output += "\n" + StringUtils.IndentPrefix(indentLevel) + $"{propertyName}\t{propertyType}";
                            }
                            indentLevel--;
                        }

                        // Navigation Properties
                        IEnumerable<OdcmProperty> navigationProperties = @class.NavigationProperties();
                        if (navigationProperties.Any())
                        {
                            output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Navigation Properties:";
                            indentLevel++;
                            foreach (OdcmProperty property in navigationProperties)
                            {
                                string propertyName = property.Name;
                                string propertyType = property.Type.FullName;
                                output += "\n" + StringUtils.IndentPrefix(indentLevel) + $"{propertyName}\t{propertyType}";
                            }
                            indentLevel--;
                        }

                        // Methods
                        if (@class.Methods.Any())
                        {
                            output += "\n" + StringUtils.IndentPrefix(indentLevel) + "Methods:";
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
                                output += "\n" + StringUtils.IndentPrefix(indentLevel) + $"{returnType} {methodName}({parameters})";
                                if (!string.IsNullOrWhiteSpace(methodDescription))
                                {
                                    output += "\n" + StringUtils.IndentPrefix(indentLevel + 1) + methodDescription;
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
