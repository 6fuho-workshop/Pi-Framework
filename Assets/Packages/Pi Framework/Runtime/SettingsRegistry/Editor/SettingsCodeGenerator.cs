using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using System;
using PF.PiEditor.Utils;


namespace PF.PiEditor.Settings
{
    /// <summary>
    /// Generate partial code for Pi class 
    /// todo: tách thành các partial class d?a trên top level settings ?? gi?m vi?c conflict
    /// </summary>
    internal class SettingsGenerator
    {
        readonly string sourceFile;
        const string classSuffix = "Settings";
        SettingsGenerator()
        {
            sourceFile = PiPath.scriptPath + "/Settings.main.cs";
        }

        [OnAssetModificationOfType(typeof(SettingDefinition))]
        static void OnAssetModification(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            Debug.Log("OnAssetModification");

            foreach (string str in importedAssets)
            {
                Debug.Log("Reimported Asset: " + str);
            }
            foreach (string str in deletedAssets)
            {
                Debug.Log("Deleted Asset: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }

            if (didDomainReload)
            {
                Debug.Log("Domain has been reloaded");
            }
        }

        public static void Generate()
        {
            var changed = new SettingsGenerator().GenerateCodeFile();
            if (changed)
                PE.Recompile();
        }

        bool GenerateCodeFile()
        {
            var success = GenerateClass();
            //StringWriter sw = new StringWriter();
            //string oldCode = File.Exists(sourceFile) ? File.ReadAllText(sourceFile) : "";
            //string newCode = sw.ToString();
            //if (!oldCode.Equals(newCode))
            //{
            //File.WriteAllText(sourceFile, newCode);
            //Debug.Log(sourceFile + " file generated!");
            //}
            return success;
        }

        bool GenerateClass()
        {
            Node root = new(string.Empty) { isRoot = true };

            //var usingDirectives = InitUsingDirectives();
            var usingDirectives = new List<string>();
   
            if (BuildSettingsTree(root, usingDirectives))
            {

                ClassModel settingsClass = CreateNodeClass(root);
                var baseClass = settingsClass.BaseClass;
                settingsClass.BaseClass = "RuntimeSettings";
                if (baseClass.Contains("IPersistentSetting"))
                    settingsClass.BaseClass += ", IPersistentSetting";
                settingsClass.IndentSize = 0;
                settingsClass.Attributes.Clear();
                settingsClass.KeyWords.Add(KeyWord.Partial);

                Method buildNodeDict = BuildNodeDictMethod(root);
                settingsClass.Methods.Add(buildNodeDict);

                FileModel file = new("Settings.main");
                file.LoadUsingDirectives(usingDirectives);
                file.Classes.Add(settingsClass);

                File.WriteAllText(sourceFile, file.ToString());

                return true;
            }

            return false;
        }

        Method BuildNodeDictMethod(Node root)
        {
            Method buildNodeDict = new Method(BuiltInDataType.Void, "BuildNodeDict");
            buildNodeDict.KeyWords.Add(KeyWord.Override);
            buildNodeDict.AccessModifier = AccessModifier.Protected;
            buildNodeDict.IndentSize -= CsGenerator.DefaultTabSize;
            var body = buildNodeDict.BodyLines;
            body.Add("_nodeDict = new Dictionary<string, ISettingNode>() {");
            var allNodes = root.GetChildrenRecursive();
            foreach (var node in allNodes)
            {
                body.Add(Util.Tab + "{\"" + node.fullPath + "\", " + node.fullPath + "},");
            }
            body.Add("};");

            return buildNodeDict;
        }

        /*
        List<string> InitUsingDirectives()
        {
            return new List<string>()
            {
                "System;",
                "UnityEngine;",
                "PF;",
                "PF.Settings;",
                "System.Collections.Generic;"
            };
        }
        */

        string GetNodeClassName(Node node)
        {
            string name = string.IsNullOrEmpty(node.name) ? "" : char.ToUpper(node.name[0]).ToString();
            if (node.name.Length > 1)
                name += node.name[1..];
            return name += classSuffix;
        }

        ClassModel CreateNodeClass(Node node)
        {
            ClassModel nodeClass = new(GetNodeClassName(node));
            nodeClass.AddAttribute(new AttributeModel("Serializable"));
            nodeClass.BaseClass = "SettingNode";
            List<Field> fields = new();
            List<Property> properties = new();

            var loadableMethod = GetLoadMethod(node);
            if (loadableMethod != null)
            {
                nodeClass.Methods.Add(loadableMethod);
                nodeClass.BaseClass += ", IPersistentSetting";
            }

            foreach (var item in node.entities)
            {
                var field = CreateField(item);
                fields.Add(field);

                var property = new Property(item.ValueType, item.LeafName);
                property.IsAutoImplemented = false;
                var fieldName = field.Name;

                if (node.isRoot)
                {
                    property.SingleKeyWord = KeyWord.Static;
                    fieldName = "_instance." + fieldName;
                }

                if (item.IsReadOnly)
                    property.ExpressionBody = fieldName;
                else
                {
                    property.GetterBody = fieldName;
                    var setterBody = $"if({fieldName} == value) return; ";
                    setterBody += fieldName + " = value; ";
                    if (node.isRoot)
                        setterBody += "_instance.";
                    setterBody += $"OnChanged(\"{item.LeafName}\")";
                    if (item.Persist)
                    {
                        string TypeName = GetTypeNameStorage(item.ValueType);
                        if (TypeName != null)
                        {
                            var name = item.LeafName;
                            string key = String.IsNullOrWhiteSpace(item.StorageKeyOverride) ? $"{node.name}.{name}" : item.StorageKeyOverride;
                            string dataStore = node.isRoot ? "_instance.dataStore" : "dataStore";
                            setterBody += $"; {dataStore}.Set{TypeName}(\"{key}\", value)";
                        }
                        //storage.SetFloat(".global2", value);
                    }
                    property.SetterBody = setterBody;
                }

                properties.Add(property);
            }

            foreach (var childNode in node.childNodes.Values)
            {
                var typeName = GetNodeClassName(childNode);
                var childClass = CreateNodeClass(childNode);
                nodeClass.NestedClasses.Add(childClass);

                var field = new Field(typeName, "_" + childNode.name);
                field.AddAttribute(new AttributeModel("SerializeField"));
                field.AccessModifier = AccessModifier.Private;
                fields.Add(field);

                var property = new Property(typeName, childNode.name);
                property.ExpressionBody = node.isRoot ? "_instance." + field.Name : field.Name;
                if (node.isRoot)
                    property.SingleKeyWord = KeyWord.Static;
                properties.Add(property);
            }
            nodeClass.Fields = fields;
            nodeClass.Properties = properties;
            return nodeClass;
        }

        /// <summary>
        /// Generate method 'OnLoadCallback' implements IPersistentSetting
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Method GetLoadMethod(Node node)
        {
            List<string> body = new();

            foreach (var item in node.entities)
            {
                if (item.IsReadOnly || !item.Persist)
                    continue;
                string TypeName = GetTypeNameStorage(item.ValueType);
                if (TypeName == null)
                    continue;

                var name = item.LeafName;
                string getType = "Get" + TypeName;
                string key = String.IsNullOrWhiteSpace(item.StorageKeyOverride) ? $"{node.name}.{name}" : item.StorageKeyOverride;
                body.Add($"{name} = dataStore.{getType}(\"{key}\", _{name});");
            }
            if (body.Count > 0)
            {
                var method = new Method(BuiltInDataType.Void, "OnLoadCallback");
                method.AccessModifier = AccessModifier.Public;
                method.IndentSize -= CsGenerator.DefaultTabSize;
                method.BodyLines = body;
                return method;
            }
            return null;
        }

        string GetTypeNameStorage(string dataType)
        {
            return dataType switch
            {
                "bool" => "Bool",
                "int" => "Int",
                "long" => "Long",
                "float" => "Float",
                "double" => "Double",
                "string" => "String",
                "byte[]" => "Bytes",
                _ => null,
            };
        }

        Field CreateField(SettingEntry item)
        {
            var field = new Field(item.ValueType, "_" + item.LeafName);
            field.AddAttribute(new AttributeModel("SerializeField"));
            if (!string.IsNullOrEmpty(item.Description))
                field.AddAttribute(new AttributeModel("Tooltip") { SingleParameter = new Parameter($"\"{item.Description}\"") });

            var rangeAttribute = GetRangeAttribute(item);
            if (rangeAttribute != null)
                field.AddAttribute(rangeAttribute);

            field.AccessModifier = AccessModifier.Private;
            field.DefaultValue = string.IsNullOrEmpty(item.DefaultExpression) ? null : item.DefaultExpression;

            return field;
        }
        AttributeModel GetRangeAttribute(SettingEntry item)
        {
            if (item.HasRange && !item.Min.Equals(item.Max))
            {
                string floatChar = item.ValueType.Equals("float") ? "f" : "";
                var rangeAttribute = new AttributeModel("Range");
                rangeAttribute.Parameters.Add(new Parameter(item.Min.ToString() + floatChar));
                rangeAttribute.Parameters.Add(new Parameter(item.Max.ToString() + floatChar));
                return rangeAttribute;
            }
            else
                return null;
        }

        bool BuildSettingsTree(Node root, List<string> usingDirectives)
        {
            string[] assetPaths = AssetUtility.FindScriptableObjects<SettingDefinition>();

            foreach (var ap in assetPaths)
            {

                var manifest = AssetDatabase.LoadAssetAtPath<SettingDefinition>(ap);
                usingDirectives.AddRange(manifest.UsingNamespaces);
                var basePath = manifest.PathPrefix.Replace(" ", "");
                if (manifest.Entries == null)
                    continue;

                foreach (var item in manifest.Entries)
                {
                    if (!item.IsValid())
                        continue;

                    var nodePath = basePath;
                    if (!string.IsNullOrEmpty(item.ParentNodePath) && !string.IsNullOrEmpty(basePath))
                        nodePath += ".";
                    nodePath += item.ParentNodePath;

                    var node = GetSettingNodeByPath(nodePath, root);
                    if (!node.AddEntity(item))
                        return false;
                }
            }

            return true;
        }
        Node GetSettingNodeByPath(string path, Node root)
        {
            if (string.IsNullOrEmpty(path))
                return root;
            var idx = path.LastIndexOf(".");
            var name = path[(idx + 1)..];
            var supPath = idx < 0 ? string.Empty : path[..idx];

            return GetSettingNodeByPath(supPath, root).GetOrCreateChild(name);
        }

        class Node
        {
            public string fullPath;
            public string name;
            public bool isRoot;
            public Node parent;
            public Dictionary<string, Node> childNodes;
            public List<SettingEntry> entities;

            public Node(string name)
            {
                fullPath = string.Empty;
                this.name = name;
                childNodes = new Dictionary<string, Node>();
                entities = new List<SettingEntry>();
            }

            public Node GetOrCreateChild(string name)
            {
                if (!childNodes.ContainsKey(name))
                {
                    var child = new Node(name);
                    child.parent = this;
                    child.fullPath = string.IsNullOrEmpty(fullPath) ? name : fullPath + "." + name;
                    childNodes[name] = child;
                    Debug.Log("Create SettingNode: " + child.fullPath);
                }
                return childNodes[name];
            }

            // retrun false if error
            public bool AddEntity(SettingEntry item)
            {
                foreach (var child in entities)
                {
                    if (child.LeafName.Equals(item.LeafName))
                    {
                        EditorUtility.DisplayDialog("Invalid Setting definition", $"Setting entity duplicated: {fullPath}.{item.LeafName}", "OK");
                        return false;
                    }


                }
                entities.Add(item);

                return true;
            }

            public HashSet<Node> GetChildrenRecursive()
            {
                var hs = new HashSet<Node>();
                foreach (var child in childNodes.Values)
                {
                    hs.Add(child);
                    hs.UnionWith(child.GetChildrenRecursive());
                }
                return hs;
            }
        }
    }
}