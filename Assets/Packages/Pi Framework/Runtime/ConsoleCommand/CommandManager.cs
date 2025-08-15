using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



namespace PF.Commands
{

    public class CommandManager
    {


        public List<IConsoleCommand> RegisteredCommands = new List<IConsoleCommand>();
        public void Init()
        {
            //Register(new CommandsCommand());
            //Register(new GpuInfoCommand());
            //Register(new HelpCommand());
            //Register(new InfoCommand());
            //Register(new RenderTypeCommand());
        }

        public void Reset() { }


        /// <summary> Returns whether a equals b, ignoring any case differences. </summary>
        bool CaselessEquals(string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);
        

        /// <summary> Returns whether a starts with b, ignoring any case differences. </summary>
		bool CaselessStarts(string a, string b) => a.StartsWith(b, StringComparison.OrdinalIgnoreCase);

        public void Register(IConsoleCommand command)
        {
            foreach (IConsoleCommand cmd in RegisteredCommands)
            {
                if (CaselessEquals(cmd.Name, command.Name))
                {
                    throw new InvalidOperationException("Another command already has name : " + command.Name);
                }
            }
            RegisteredCommands.Add(command);
        }

        public IConsoleCommand GetMatchingCommand(string commandName)
        {
            bool matchFound = false;
            IConsoleCommand matchingCommand = null;
            foreach (IConsoleCommand cmd in RegisteredCommands)
            {
                if (CaselessStarts(cmd.Name, commandName))
                {
                    if (matchFound)
                    {
                        //game.Chat.Add("&e/client: Multiple commands found that start with: \"&f" + commandName + "&e\".");
                        return null;
                    }
                    matchFound = true;
                    matchingCommand = cmd;
                }
            }

            if (matchingCommand == null)
            {
                //game.Chat.Add("&e/client: Unrecognised command: \"&f" + commandName + "&e\".");
            }
            return matchingCommand;
        }

        public void Execute(string text)
        {
            CommandReader reader = new CommandReader(text);
            if (reader.TotalArgs == 0)
            {
                //game.Chat.Add("&eList of client commands:");
                //PrintDefinedCommands(game);
                PrintDefinedCommands();
                //game.Chat.Add("&eTo see a particular command's help, type /client help [cmd name]");
                return;
            }
            string commandName = reader.Next();
            IConsoleCommand cmd = GetMatchingCommand(commandName);
            if (cmd != null)
            {
                cmd.Execute(reader);
            }
        }

        public void PrintDefinedCommands()
        {
            List<string> lines = new List<string>();
            StringBuilder buffer = new StringBuilder(64);
            foreach (IConsoleCommand cmd in RegisteredCommands)
            {
                string name = cmd.Name;
                if (buffer.Length + name.Length > 64)
                {
                    lines.Add(buffer.ToString());
                    buffer.Length = 0;
                }
                buffer.Append(name);
                buffer.Append(", ");
            }
            if (buffer.Length > 0)
                lines.Add(buffer.ToString());
            foreach (string part in lines)
            {
                //game.Chat.Add(part);
            }
        }

        public void Dispose()
        {
            RegisteredCommands.Clear();
        }
    }
}
