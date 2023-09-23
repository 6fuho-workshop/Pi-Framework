using UnityEngine;
using UnityEditor;
using PiEditor.Utils;
using System.Collections.Generic;
using System.IO;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using System;

namespace PiEditor.Settings
{
    /// <summary>
    /// Generate partial code for Pi class 
    /// </summary>
    internal class SettingsGenerator
    {
        readonly string sourceFile;
        const string classSuffix = "Settings";
        SettingsGenerator()
        {
            sourceFile = FileHelper.GetPiDataPath() + "/Code/Settings.cs";
        }

        [MenuItem("Pi/Force Generate Code/Settings")]
        public static void Generate()
        {
            new SettingsGenerator().GenerateCodeFile();
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
            SettingNode root = new SettingNode(string.Empty);
            var usingDirectives = new List<string>(); //collect all usingDirectives
            usingDirectives.Add("System;");
            usingDirectives.Add("UnityEngine;");

            BuildSettingsTree(root, usingDirectives);

            ClassModel settingsClass = CreateNodeClass(root);
            settingsClass.BaseClass = "ScriptableObject";
            settingsClass.IndentSize = 0;
            settingsClass.Attributes.Clear();

            FileModel file = new FileModel("Settings");
            file.LoadUsingDirectives(usingDirectives);
            file.Classes.Add(settingsClass);

            File.WriteAllText(sourceFile, file.ToString());
        }

        ClassModel CreateNodeClass(SettingNode node)
        {
            ClassModel nodeClass = new ClassModel(node.name+ classSuffix);
            nodeClass.AddAttribute(new AttributeModel("Serializable"));
            List<Field> fields = new List<Field>();
            List<Property> properties = new List<Property>();

            var changeEvent = new Field("Action", "settingsChanged");
            changeEvent.KeyWords.Add(KeyWord.Event);
            changeEvent.AccessModifier = AccessModifier.Public;
            fields.Add(changeEvent);
            var tab = new String(' ', CsGenerator.DefaultTabSize);

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
                property.GetterBody = field.Name;
                var setterBody = $"if({field.Name} == value) return; ";
                setterBody += field.Name + " = value; if(settingsChanged != null) settingsChanged.Invoke()";
                property.SetterBody = setterBody;
                properties.Add(property);
            }

            foreach (var childNode in node.nodeDict.Values)
            {
                var childClass = CreateNodeClass(childNode);
                //childClass.IndentSize = nodeClass.IndentSize + CsGenerator.DefaultTabSize;
                nodeClass.NestedClasses.Add(childClass);
                var field = new Field(childNode.name + classSuffix, childNode.name);
                field.AccessModifier = AccessModifier.Public;
                fields.Add(field);
            }

            nodeClass.Fields = fields;
            nodeClass.Properties = properties;
            return nodeClass;
        }

        void BuildSettingsTree(SettingNode root, List<string> usingDirectives)
        {
            string[] assetPaths = FileHelper.FindScriptableObjects<SettingsManifest>();
            List<SettingsManifest> descs = new List<SettingsManifest>();

            foreach (var ap in assetPaths)
            {
                var des = AssetDatabase.LoadAssetAtPath<SettingsManifest>(ap);
                usingDirectives.AddRange(des.usingDirectives);
                var path = des.basePath.Replace(" ", "");
                var node = GetSettingNodeByPath(path, root);
                node.items.AddRange(des.settingItems);
                foreach (var branch in des.branches)
                {
                    node.AddBranch(branch);
                }
            }
        }

        //void AddBranchToNode(SettingsDescription.Branch branch, )
        SettingNode GetSettingNodeByPath(string path, SettingNode root)
        {
            if (string.IsNullOrEmpty(path))
                return root;
            var idx = path.LastIndexOf(".");
            var name = path.Substring(idx + 1);
            var supPath = idx < 0 ? string.Empty : path.Substring(0, idx);

            return GetSettingNodeByPath(supPath, root).GetOrCreateChild(name);
        }

        class SettingNode
        {
            public string fullPath;
            public string name;
            public SettingNode parent;
            public Dictionary<string, SettingNode> nodeDict;
            public List<SettingItem> items;

            public SettingNode(string name)
            {
                fullPath = string.Empty;
                this.name = name;
                nodeDict = new Dictionary<string, SettingNode>();
                items = new List<SettingItem>();
            }

            public SettingNode GetOrCreateChild(string name)
            {
                if (!nodeDict.ContainsKey(name))
                {
                    var child = new SettingNode(name);
                    child.parent = this;
                    child.fullPath = string.IsNullOrEmpty(fullPath) ? name : fullPath + "." + name;
                    nodeDict[name] = child;
                    Debug.Log("Create SettingNode: " + child.fullPath);
                }
                return nodeDict[name];
            }

            public void AddBranch(SettingsManifest.Branch branch)
            {
                var child = GetOrCreateChild(branch.name);
                child.items.AddRange(branch.settingItems);
                foreach (var subBranch in branch.branches)
                {
                    child.AddBranch(subBranch);
                }
            }
        }
    }
}