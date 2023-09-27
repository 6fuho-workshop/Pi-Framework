using UnityEngine;
using UnityEditor;
using PiEditor.Utils;
using System.Collections.Generic;
using System.IO;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using System;
using System.Security.Cryptography;
using static UnityEditor.Progress;
using UnityEditor.Experimental.GraphView;
using PiFramework.Settings;

namespace PiEditor.Settings
{
    /// <summary>
    /// Generate partial code for Pi class 
    /// todo: tách thành các partial class dựa trên top level settings để giảm việc conflict
    /// </summary>
    internal class Settings_Generator
    {
        readonly string sourceFile;
        const string classSuffix = "Settings";
        Settings_Generator()
        {
            sourceFile = FileHelper.GetPiDataPath() + "/Settings/Settings.main.cs";
        }

        [MenuItem("Pi/Force Generate Code/Settings")]
        public static void Generate()
        {
            new Settings_Generator().GenerateCodeFile();
        }

        void GenerateCodeFile()
        {
            GenerateClass();
            //StringWriter sw = new StringWriter();
            //string oldCode = File.Exists(sourceFile) ? File.ReadAllText(sourceFile) : "";
            //string newCode = sw.ToString();
            //if (!oldCode.Equals(newCode))
            //{
            //File.WriteAllText(sourceFile, newCode);
            //Debug.Log(sourceFile + " file generated!");
            //}
        }

        void GenerateClass()
        {
            SettingNode root = new(string.Empty) { isRoot = true };
            //collect all usingDirectives
            List<string> usingDirectives = new()
            {
                "System;",
                "UnityEngine;"
            }; 

            BuildSettingsTree(root, usingDirectives);

            ClassModel settingsClass = CreateNodeClass(root);
            settingsClass.BaseClass = "ScriptableObject";
            settingsClass.IndentSize = 0;
            settingsClass.Attributes.Clear();
            settingsClass.KeyWords.Add(KeyWord.Partial);

            FileModel file = new("Settings");
            file.LoadUsingDirectives(usingDirectives);
            file.Classes.Add(settingsClass);

            File.WriteAllText(sourceFile, file.ToString());
        }

        ClassModel CreateNodeClass(SettingNode node)
        {
            ClassModel nodeClass = new(node.name + classSuffix);
            nodeClass.AddAttribute(new AttributeModel("Serializable"));
            List<Field> fields = new();
            List<Property> properties = new();

            if (CanHaveChangedEvent(node) && !node.isRoot)
            {
                var changeEvent = new Field("Action", "changed");
                changeEvent.KeyWords.Add(KeyWord.Event);
                changeEvent.AccessModifier = AccessModifier.Public;
                fields.Add(changeEvent);
            }

            foreach (var item in node.items)
            {
                var field = new Field(item.type, "_" + item.name);
                field.AddAttribute(new AttributeModel("SerializeField"));
                if (!string.IsNullOrEmpty(item.tooltip))
                    field.AddAttribute(new AttributeModel("Tooltip") { SingleParameter = new Parameter($"\"{item.tooltip}\"") });
                if (!item.rangeFrom.Equals(item.rangeTo))
                {
                    var rangeAttribute = new AttributeModel("Range");
                    rangeAttribute.Parameters.Add(new Parameter(item.rangeFrom.ToString()));
                    rangeAttribute.Parameters.Add(new Parameter(item.rangeTo.ToString()));
                    field.AddAttribute(rangeAttribute);
                }
                field.AccessModifier = AccessModifier.Private;
                field.DefaultValue = string.IsNullOrEmpty(item.defaultValue) ? null : item.defaultValue;
                fields.Add(field);

                var property = new Property(item.type, item.name);
                property.IsAutoImplemented = false;
                var fieldName = field.Name;
                if (node.isRoot)
                {
                    property.SingleKeyWord = KeyWord.Static;
                    fieldName = "_instance." + fieldName;
                }
                property.GetterBody = fieldName;
                if (!item.readOnly)
                {
                    var setterBody = $"if({fieldName} == value) return; ";
                    setterBody += fieldName + " = value; ";
                    setterBody += "changed?.Invoke()";
                    property.SetterBody = setterBody;
                }

                properties.Add(property);
            }

            foreach (var childNode in node.childNodes.Values)
            {
                var typeName = childNode.name + classSuffix;
                var childClass = CreateNodeClass(childNode);
                //childClass.IndentSize = nodeClass.IndentSize + CsGenerator.DefaultTabSize;
                nodeClass.NestedClasses.Add(childClass);
                var field = new Field(typeName, "_" + childNode.name);
                field.AddAttribute(new AttributeModel("SerializeField"));
                field.AccessModifier = AccessModifier.Private;
                fields.Add(field);

                var property = new Property(typeName, childNode.name);
                property.GetterBody = field.Name;
                if (node.isRoot)
                {
                    property.SingleKeyWord = KeyWord.Static;
                    property.GetterBody = "_instance." + field.Name;
                }
                property.IsAutoImplemented = false;
                properties.Add(property);

                
            }

            nodeClass.Fields = fields;
            nodeClass.Properties = properties;
            return nodeClass;
        }

        bool CanHaveChangedEvent(SettingNode node)
        {
            foreach(var item in node.items)
            {
                if (!item.readOnly)
                    return true;
            }
            return false;
        }

        void BuildSettingsTree(SettingNode root, List<string> usingDirectives)
        {
            string[] assetPaths = FileHelper.FindScriptableObjects<SettingsManifest>();

            foreach (var ap in assetPaths)
            {

                var manifest = AssetDatabase.LoadAssetAtPath<SettingsManifest>(ap);
                usingDirectives.AddRange(manifest.usingDirectives);
                var basePath = manifest.basePath.Replace(" ", "");
                if (manifest.settingItems == null)
                    continue;

                foreach (var item in manifest.settingItems)
                {
                    if (!item.Validate())
                        continue;

                    var nodePath = basePath;
                    if (!string.IsNullOrEmpty(item.nodePath) && !string.IsNullOrEmpty(basePath))
                        nodePath += ".";
                    nodePath += item.nodePath;

                    var node = GetSettingNodeByPath(nodePath, root);
                    node.items.Add(item);
                }
            }
        }

        SettingNode GetSettingNodeByPath(string path, SettingNode root)
        {
            if (string.IsNullOrEmpty(path))
                return root;
            var idx = path.LastIndexOf(".");
            var name = path[(idx + 1)..];
            var supPath = idx < 0 ? string.Empty : path[..idx];

            return GetSettingNodeByPath(supPath, root).GetOrCreateChild(name);
        }

        class SettingNode
        {
            public string fullPath;
            public string name;
            public bool isRoot;
            public SettingNode parent;
            public Dictionary<string, SettingNode> childNodes;
            public List<SettingItem> items;

            public SettingNode(string name)
            {
                fullPath = string.Empty;
                this.name = name;
                childNodes = new Dictionary<string, SettingNode>();
                items = new List<SettingItem>();
            }

            public SettingNode GetOrCreateChild(string name)
            {
                if (!childNodes.ContainsKey(name))
                {
                    var child = new SettingNode(name);
                    child.parent = this;
                    child.fullPath = string.IsNullOrEmpty(fullPath) ? name : fullPath + "." + name;
                    childNodes[name] = child;
                    Debug.Log("Create SettingNode: " + child.fullPath);
                }
                return childNodes[name];
            }

            public void AddItem(SettingItem item)
            {
                items.Add(item);
            }
        }
    }
}