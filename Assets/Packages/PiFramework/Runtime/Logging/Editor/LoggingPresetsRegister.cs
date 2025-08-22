// Package/Logging/Editor/LoggingPresetsHook.cs
using PF.PiEditor.ControlPanel.Presets;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PF.Logging;
using PF.Logging.Internal;
using PF.Contracts;

namespace PF.PiEditor.ControlPanel
{

    public static class LoggingPresetsRegister
    {
        [InitializeOnLoadMethod]
        private static void RegisterLogCategoryTreePreset()
        {
            PresetTypeRegistry.Register(
                soType: typeof(LogCategoryTree),
                displayName: "Log Category Tree",
                createDefault: CreateDefaultLogCategoryTree,
                order: 0
            );
        }

        private static ScriptableObject CreateDefaultLogCategoryTree()
        {
            var so = ScriptableObject.CreateInstance<LogCategoryTree>();
            so.Root = new LogCategoryTree.Node
            {
                Name = "PF",
                Level = LogLevel.Inherit,
                Children = new List<LogCategoryTree.Node>()
            };
            return so;
        }
    }
}