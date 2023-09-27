using UnityEngine;
using UnityEditor;

using PiEditor.Utils;
using PiFramework;

using System.Collections;
using System.Collections.Generic;
using System;
using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace PiEditor
{
    /// <summary>
    /// Generate partial code for Pi class 
    /// </summary>
    internal class PinServicesGenerator
    {
        string sourceFile;
        CodeCompileUnit compileUnit;
        CodeNamespace nameSpace;
        CodeTypeDeclaration piClass;
        //public List<PinServiceAttribute> IndexServiceAttributes;
        PinServicesGenerator()
        {
            sourceFile = FileHelper.GetPiDataPath() + "/PiClass/Pi.pinService.cs";
            compileUnit = new CodeCompileUnit();
        }

        [MenuItem("Pi/Force Generate Code/PinServices")]
        public static void Generate()
        {
            new PinServicesGenerator().GenerateCodeFile();
        }

        void GenerateCodeFile()
        {
            GenerateClass();
            GenerateServices();

            var options = new CodeGeneratorOptions();
            options.VerbatimOrder = true;

            StringWriter sw = new StringWriter();

            new CSharpCodeProvider().GenerateCodeFromCompileUnit(compileUnit, sw, options);
            string oldCode = File.Exists(sourceFile) ? File.ReadAllText(sourceFile) : "";
            string newCode = sw.ToString();
            if (!oldCode.Equals(newCode))
            {
                File.WriteAllText(sourceFile, newCode);
                Debug.Log(sourceFile + " file generated!");
            }
        }

        void GenerateClass()
        {
            nameSpace = new CodeNamespace();
            compileUnit.Namespaces.Add(nameSpace);
            nameSpace.Imports.Add(new CodeNamespaceImport("System"));

            piClass = new CodeTypeDeclaration("Pi");
            piClass.IsPartial = true;
            piClass.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            nameSpace.Types.Add(piClass);// Add the new type to the namespace type collection.
        }

        void GenerateServices_old()
        {/*
            var i = 0;
            foreach (var attr in IndexServiceAttributes)
            {
                i++;
                string fieldName = "_" + attr.Name;

                CodeSnippetTypeMember field = new CodeSnippetTypeMember($"    private static {attr.Type.FullName} {fieldName};");
                field.Comments.Add(new CodeCommentStatement("Pin Service for quick reference. Package: " + attr.PackageName));
                piClass.Members.Add(field);

                //CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(attr.Type, "__" + attr.Name);
                CodeMemberProperty serviceProperty = new CodeMemberProperty();
                if (i == 1)
                {
                    field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Service Pinning Region"));
                }
                if (i == IndexServiceAttributes.Count)
                {
                    serviceProperty.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
                }

                serviceProperty.Type = new CodeTypeReference(attr.Type);
                serviceProperty.Name = attr.Name;
                serviceProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;


                CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(fieldName),
                    CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));

                CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(
                                new CodeVariableReferenceExpression("serviceLocator"), "GetService", new CodeTypeReference(attr.Type));
                CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(method, new CodeParameterDeclarationExpression[] { });

                CodeConditionStatement conditionalStatement = new CodeConditionStatement(condition,
                    new CodeStatement[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName), invoke) });
                serviceProperty.GetStatements.Add(conditionalStatement);

                serviceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fieldName)));

                piClass.Members.Add(serviceProperty);
            }
            */
        }
            

        void GenerateServices()
        {
            var modules = ModuleSetup.GetAllModules();
            foreach (var module in modules)
            {
                foreach (var pin in module.pinServices)
                {
                    //if(!string.IsNullOrEmpty(pin.usingNameSpace))
                    //nameSpace.Imports.Add(new CodeNamespaceImport(pin.usingNameSpace));

                    string fieldName = "_" + pin.name;
                    CodeSnippetTypeMember field = new CodeSnippetTypeMember($"    private static {pin.fullType} {fieldName};");
                    field.Comments.Add(new CodeCommentStatement("Pin Service for quick reference. Package: " + module.displayName));
                    piClass.Members.Add(field);

                    CodeMemberProperty serviceProperty = new CodeMemberProperty();
                    serviceProperty.Type = new CodeTypeReference(pin.fullType);
                    serviceProperty.Name = pin.name;
                    serviceProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;


                    CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(fieldName),
                        CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));

                    CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(
                                    new CodeVariableReferenceExpression("serviceLocator"), "GetService", new CodeTypeReference(pin.fullType));
                    CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(method, new CodeParameterDeclarationExpression[] { });

                    CodeConditionStatement conditionalStatement = new CodeConditionStatement(condition,
                        new CodeStatement[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName), invoke) });
                    serviceProperty.GetStatements.Add(conditionalStatement);

                    serviceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fieldName)));

                    piClass.Members.Add(serviceProperty);

                }
            }
        }
    }
}