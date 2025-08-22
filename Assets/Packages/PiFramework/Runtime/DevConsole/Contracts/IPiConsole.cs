using System;

namespace PF.Contracts
{
    public delegate void CommandExecForResult(PiConsoleResult result);

    public interface IPiConsole
    {
        void Exec(string command);
        PiConsoleResult ExecForResult(string command);
        void RegisterCommand(string name, CommandExecForResult executioner);
        void RegisterCommand(string name, Action executioner);
    }
}