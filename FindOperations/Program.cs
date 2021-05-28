using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Compiler;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Learning.Strategies;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.VersionSpace;

namespace FindOperations
{
    internal class Program
    {
        private static readonly Grammar Grammar = DSLCompiler.Compile(new CompilerOptions
        {
            InputGrammarText = File.ReadAllText("synthesis/grammar/Findops.grammar"),
            References = CompilerReference.FromAssemblyFiles(typeof(Program).GetTypeInfo().Assembly)
        }).Value;

        private static SynthesisEngine _prose;

        private static readonly Dictionary<State, object> Examples = new Dictionary<State, object>();
        
        private static void Main(string[] args)
        {
            _prose = ConfigureSynthesis();
            var menu = @"Select one of the options: 
1 - provide new example
2 - exit";
            var option = 0;
            while (option != 2)
            {
                Console.Out.WriteLine(menu);
                try
                {
                    option = short.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Invalid option. Try again.");
                    continue;
                }

                try
                {
                    RunOption(option);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Something went wrong...");
                    Console.Error.WriteLine("Exception message: {0}", e.Message);
                }
            }
        }
        
        private static void RunOption(int option)
        {
            switch (option)
            {
                case 1:
                    LearnFromNewExample();
                    break;
                default:
                    Console.Out.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
        
        private static void LearnFromNewExample()
        {
            Console.Out.Write("After running it once please press 2.\nProvide a new input-output example (e.g., {1,2,3,4},7 ): ");
            try
            {
                string input = Console.ReadLine();
                if (input != null)
                {
                    int startFirstExample = input.IndexOf('{');
                    int endFirstExample = input.IndexOf('}');
                    int startSecondExample = input.IndexOf(',', endFirstExample);
                    int endSecondExample = input.Length-1;

                    if (startFirstExample+1 >= endFirstExample || startSecondExample >= endSecondExample)
                        throw new Exception(
                            "Invalid example format. Please try again.");

                    string inputTemp = input.Substring(startFirstExample+1,endFirstExample-startFirstExample-1);
                    string[] inputArr = (inputTemp.Split(','));
                    List<int> inputExample = new List<int>();
                    foreach (string temp in inputArr) {
                        inputExample.Add(int.Parse(temp));
                    }

                    int outputExample = int.Parse(input.Substring(startSecondExample+1,endSecondExample-startSecondExample));
                    
                    State inputState = State.CreateForExecution(Grammar.InputSymbol, inputExample);
                    Examples.Add(inputState, outputExample);
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid example format. Please try again.");
            }

            var spec = new ExampleSpec(Examples);
            Console.Out.WriteLine("Learning a program for examples:");
            foreach (KeyValuePair<State, object> example in Examples)
                Console.WriteLine("\"{0}\" -> \"{1}\"", example.Key.Bindings.First().Value, example.Value);

            var scoreFeature = new RankingScore(Grammar);

            //learn the set of topmost K ranked programs that satisfy the spec 
            //K's value is specified in the third parameter of LearnGrammarTopK and can be changed as required
            ProgramSet topPrograms = _prose.LearnGrammarTopK(spec, scoreFeature, 3, null);
            if (topPrograms.IsEmpty)
                throw new Exception("No program was found for this specification.");

            Console.Out.WriteLine("Top programs till rank 3:");
            var counter = 1;
            foreach (ProgramNode program in topPrograms.RealizedPrograms)
            {
                if (counter > 50) break;
                Console.Out.WriteLine("==========================");
                Console.Out.WriteLine("Program {0}: ", counter);
                Console.Out.WriteLine(program.PrintAST(ASTSerializationFormat.HumanReadable));
                counter++;
            }
        }

        public static SynthesisEngine ConfigureSynthesis()
        {
            var witnessFunctions = new WitnessFunctions(Grammar);
            var deductiveSynthesis = new DeductiveSynthesis(witnessFunctions);
            var synthesisExtrategies = new ISynthesisStrategy[] {deductiveSynthesis};
            var synthesisConfig = new SynthesisEngine.Config {Strategies = synthesisExtrategies};
            var prose = new SynthesisEngine(Grammar, synthesisConfig);
            return prose;
        }
        
    }
}