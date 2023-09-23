using UnityEngine;
using System;
using System.Collections.Generic;
using PiFramework;

namespace PiFramework
{

    public delegate void CommandExecForResult(PiConsoleResult result);

    public class PiConsole
    {
        Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        Dictionary<string, CommandExecForResult> _cmdExecs = new Dictionary<string, CommandExecForResult>();
        
        internal PiConsole() { }

        public void Exec(string command)
        {
            var name = GetCommandName(command.Trim());
            Action act;
            if (_actions.TryGetValue(name, out act))
            {
                act.Invoke();
            }
            else
            {
                CommandExecForResult cmd;
                if (_cmdExecs.TryGetValue(name, out cmd))
                {
                    cmd.Invoke(new PiConsoleResult());
                }
                else // command not found
                {
                    Debug.LogError("Command [" + name + "] not found");
                }
            }
        }

        public PiConsoleResult ExecForResult(string command)
        {
            var result = new PiConsoleResult();
            var name = GetCommandName(command.Trim());
            Action act;
            if (_actions.TryGetValue(name, out act))
            {
                act.Invoke();
                result.IsDone = true;
            }
            else
            {
                CommandExecForResult cmd;
                if (_cmdExecs.TryGetValue(name, out cmd))
                {
                    cmd.Invoke(result);
                }
                else // command not found
                {
                    result.Error = true;
                    result.Code = -1;
                    result.Message = "Command [" + name + "] not found";
                    result.IsDone = true;
                    Debug.LogError("Command [" + name + "] not found");
                }
            }
            return result;
        }

        string GetCommandName(string command)
        {
            var spaceIdx = command.IndexOf(' ');
            if (spaceIdx > 0)
                return command.Substring(0, spaceIdx);
            else
                return command;
        }

        public void RegisterCommand(string name, CommandExecForResult executioner)
        {
            if (_actions.ContainsKey(name) || _cmdExecs.ContainsKey(name))
            {
                Debug.LogError("Command name [" + name + "] already existed!");
            }
            else
            {
                _cmdExecs[name] = executioner;
            }
        }

        public void RegisterCommand(string name, Action executioner)
        {
            if (_actions.ContainsKey(name) || _cmdExecs.ContainsKey(name))
            {
                Debug.LogError("Command name [" + name + "] already existed!");
            }
            else
            {
                _actions[name] = executioner;
            }
        }
    }
}