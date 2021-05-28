using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Compiler;
using Microsoft.ProgramSynthesis.Diagnostics;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Learning.Strategies;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.VersionSpace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FindOperations
{
    [TestClass]
    public class FindOpsTest
    {
        private const string GrammarPath = @"C:\Users\Aditya Sharma\Source\Repos\prose_list\FindOperations\synthesis\grammar\Findops.grammar";

        [TestMethod]
        public void TestFindopElement()
        {
            //parse grammar file 
            Result<Grammar> grammar = CompileGrammar();
            //configure the prose engine 
            SynthesisEngine prose = ConfigureSynthesis(grammar.Value);

            //create the example
            //List and desired output can be assigned here according to the test input required

            //List<int> list_input = new List<int> { 10, 12, 14, 20 };
            //int desired_output = 26;

            List<int> list_input = new List<int> { 2, 3, 4, 6, 8};
            int desired_output = 20;

            //List<int> list_temp = new List<int> { 0, 0, 0 };
            //int desired_output = 0;

            //List<int> list_temp = new List<int> { 0,0,0};
            //int desired_output = 2;

            //List<int> list_temp = new List<int> { 1,1,1 };
            //int desired_output = 5;

            //List<int> list_temp = new List<int> { 0,1,2,3,4};
            //int desired_output = 5;

            //List<int> list_temp = new List<int> { 1,2,3,4};
            //int desired_output = 5;

            //List<int> list_temp = new List<int> { 2,6,7};
            //int desired_output = 21;


            State input = State.CreateForExecution(grammar.Value.InputSymbol,list_input);
            var examples = new Dictionary<State, object> {{input, desired_output}};
            var spec = new ExampleSpec(examples);

            
            //Obtain scoreFeature from parsed grammar
            var scoreFeature = new RankingScore(grammar.Value);
            
            //learn the set of topmost K ranked programs that satisfy the spec 
            //K's value is specified in the third parameter of LearnGrammarTopK and can be changed as required
            ProgramSet learnedSet = prose.LearnGrammarTopK(spec,scoreFeature,3);
            
            //Extract the learned set of programs in IEnumerable<> object
            IEnumerable<ProgramNode> programs = learnedSet.RealizedPrograms;
            if (programs.Count<ProgramNode>() == 0)
                throw new Exception("No program was found for this specification.");

            //run the first synthesized program in the same input and check if 
            //the output is correct. Optionally, output of all the learned programs can be 
            //checked by traversing through each of them in a loop
            var output = programs.First().Invoke(input) as int?;
            Assert.AreEqual(desired_output, output);

            Console.WriteLine("\nGiven list :");
            Console.WriteLine(string.Join(",", list_input));

            Console.WriteLine("\nDesired output :");
            Console.Write(output);
            
            Console.WriteLine("\n\nFollowing programs are generated :\n");
            foreach (ProgramNode temp in programs)
                Console.WriteLine(temp);
        }

        public static SynthesisEngine ConfigureSynthesis(Grammar grammar)
        {
            var witnessFunctions = new WitnessFunctions(grammar);
            var deductiveSynthesis = new DeductiveSynthesis(witnessFunctions);
            var synthesisExtrategies = new ISynthesisStrategy[] {deductiveSynthesis};
            var synthesisConfig = new SynthesisEngine.Config {Strategies = synthesisExtrategies};
            var prose = new SynthesisEngine(grammar, synthesisConfig);
            return prose;
        }

        private static Result<Grammar> CompileGrammar()
        {
            return DSLCompiler.Compile(new CompilerOptions
            {
                InputGrammarText = File.ReadAllText(GrammarPath),
                References = CompilerReference.FromAssemblyFiles(typeof(Semantics).GetTypeInfo().Assembly)
            });
        }
    }
}