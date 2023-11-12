using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using PiFramework;

namespace PiFramework.Commands
{

    /// <summary> Represents a client side action that optionally accepts arguments. </summary>
    public interface IConsoleCommand
    {
        /// <summary> Full command name, note that the user does not 
        /// have to fully type this into chat. </summary>
        string Name { get; set; }

        /// <summary> Provides help about the purpose and syntax of this 
        /// command. Can use colour codes. </summary>
        string[] Help { get; set; }

        void Execute(CommandReader reader);
    }
}