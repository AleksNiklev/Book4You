﻿using Autofac;
using LibrarySystem.ConsoleClient.Commands.Contracts;
using LibrarySystem.ConsoleClient.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibrarySystem.ConsoleClient.Core
{
    public class Engine : IEngine
    {
        private readonly IRenderer renderer;
        private readonly ICommandProcessor processor;

        public Engine(IRenderer renderer, ICommandProcessor processor)
        {
            this.renderer = renderer;
            this.processor = processor;
        }
        public void Start()
        {
            while (true)
            {
                var commandResults = new List<string>();

                try
                {
                    var commands = this.renderer.Input();
                    commandResults = this.processor.ProcessCommands(commands).ToList();
                }
                catch (Exception ex)
                {
                    commandResults.Add(ex.Message);
                }
                this.renderer.Output(commandResults);
            }
        }
        //private IEnumerable<string> ProcessCommands(IEnumerable<string> commands)
        //{
        //    var results = new List<string>();
        //    foreach (string commandLine in commands)
        //    {
        //        var splittedCommand = commandLine.Split(",").Select(c => c.Trim()).ToList();
        //        var commandName = splittedCommand[0].ToLower();
        //        var commandParams = splittedCommand.Skip(1).ToList();
        //        var command = this.autofacContext.ResolveNamed<ICommand>(commandName);

        //       results.Add(command.Execute(commandParams));
        //    }
        //    return results;
        //}
    }
}

