﻿using Antlr4.Runtime.Tree;
using LCT.Analysis;
using LCT.Generation;
using LCT.Generation.Preparation.Intermediate;
using LCT.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT
{
    /// <summary>
    /// Used to uphold things in memory during a session.
    /// TODO: Add Error Handling
    /// </summary>
    abstract public class ExecEnvironment
    {
        public ExecEnvironment()
        {
            DefinedLists = new LctUniqueList();
        }

        protected LctUniqueList DefinedLists { get; set; }

        public void Execute(string inputStatement)
        {
            string outputText = string.Empty;

            Analyzer analyser = new Analyzer(inputStatement);
            IParseTree parseTree = analyser.Analyse();
            Generator generator = new Generator(parseTree);
            Statement statement = generator.Evaluate();

            if (statement.ListDefinitions != null)
            {
                foreach(var list in statement.ListDefinitions)
                {
                    this.DefinedLists.AddOrReplace(list);
                }
            }
            else if (statement.ListsShow != null)
            {
                outputText = OutputDefinedLists();
            }

            this.Output(outputText);
        }

        /// <summary>
        /// Output can be overriden for different environments
        /// </summary>
        /// <param name="outputText"></param>
        abstract protected void Output(string outputText);

        /// <summary>
        /// String representation can be overridden
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        virtual protected string OutputDefinedLists(params string[] parameter)
        {
            StringBuilder outputBuilder = new StringBuilder();

            foreach (var list in this.DefinedLists)
            {
                outputBuilder.Append(list.Name);
                outputBuilder.Append(" = [");
                outputBuilder.Append(string.Join(",", list.Elements));
                outputBuilder.Append("]");
                outputBuilder.Append(Environment.NewLine);
            }

            return outputBuilder.ToString();
        }
    }


    public class  ConsoleExecEnvironment : ExecEnvironment
    {
        protected override void Output(string outputText)
        {
            Console.WriteLine(outputText);
        }
    }
}

