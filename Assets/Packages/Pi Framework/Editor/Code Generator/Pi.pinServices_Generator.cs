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
using PiEditor.Callbacks;

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
            sourceFile = PiPath.scriptPath + "/Pi.pinService.cs";
            compileUnit = new CodeCompileUnit();
        }

        [MenuItem("Pi/Force Generate Code/PinServices")]
        [OnLoadPiEditor]
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
                Debug.Log("oldCode:" + oldCode);
                Debug.Log("oldCodeLen:" + oldCode.Length);
                Debug.Log("newCode:" + newCode);
                Debug.Log("newCodeLen:" + newCode.Length);
                File.WriteAllText(sourceFile, newCode);
                AssetDatabase.ImportAsset("Assets" + sourceFile.Substring(Application.dataPath.Length));
                Debug.Log(sourceFile + " file generated!");

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                PE.Recompile();
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

        void GenerateServices()
        {
            var modules = ModuleSetup.GetAllModules();
            foreach (var module in modules)
            {
                foreach (var pin in module.pinServices)
                {
                    //string fieldName = "_" + pin.name;
                    //CodeSnippetTypeMember field = new CodeSnippetTypeMember($"    private static {pin.fullType} {fieldName};");
                    //field.Comments.Add(new CodeCommentStatement("Module: " + module.displayName));
                    //piClass.Members.Add(field);

                    CodeMemberProperty serviceProperty = new();
                    serviceProperty.Comments.Add(new CodeCommentStatement("Module: " + module.displayName));
                    serviceProperty.Type = new CodeTypeReference(pin.fullType);
                    serviceProperty.Name = pin.name;
                    serviceProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;


                    //CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(fieldName),
                    //CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));

                    CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(
                                    new CodeVariableReferenceExpression("services"), "GetService", new CodeTypeReference(pin.fullType));
                    CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(method, new CodeParameterDeclarationExpression[] { });

                    //CodeConditionStatement conditionalStatement = new CodeConditionStatement(condition,
                    //  new CodeStatement[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName), invoke) });
                    //serviceProperty.GetStatements.Add(conditionalStatement);

                    serviceProperty.GetStatements.Add(new CodeMethodReturnStatement(invoke));

                    piClass.Members.Add(serviceProperty);

                }
            }
        }
        /* using field to cache => need to destroy when remove Serivce and quit game
        void GenerateServices()
        {
            var modules = ModuleSetup.GetAllModules();
            foreach (var module in modules)
            {
                foreach (var pin in module.pinServices)
                {
                    string fieldName = "_" + pin.name;
                    CodeSnippetTypeMember field = new CodeSnippetTypeMember($"    private static {pin.fullType} {fieldName};");
                    field.Comments.Add(new CodeCommentStatement("Module: " + module.displayName));
                    piClass.Members.Add(field);

                    CodeMemberProperty serviceProperty = new CodeMemberProperty();
                    serviceProperty.Type = new CodeTypeReference(pin.fullType);
                    serviceProperty.Name = pin.name;
                    serviceProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;


                    CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(fieldName),
                        CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));

                    CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(
                                    new CodeVariableReferenceExpression("services"), "GetService", new CodeTypeReference(pin.fullType));
                    CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(method, new CodeParameterDeclarationExpression[] { });

                    CodeConditionStatement conditionalStatement = new CodeConditionStatement(condition,
                        new CodeStatement[] { new CodeAssignStatement(new CodeVariableReferenceExpression(fieldName), invoke) });
                    serviceProperty.GetStatements.Add(conditionalStatement);

                    serviceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(fieldName)));

                    piClass.Members.Add(serviceProperty);

                }
            }
        }
        */
    }
}