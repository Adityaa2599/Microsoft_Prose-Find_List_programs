using System.Collections.Generic;
using System.Linq;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;

namespace FindOperations
{
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar)
        {
        }
        
        [WitnessFunction(nameof(Semantics.Add), 0)]
        public DisjunctiveExamplesSpec Witnessaddop1(GrammarRule rule, ExampleSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var possible = new List<int>();
                //Domain of op1 is restricted to [1, output]
                //0 is not included in op1 because it would only lead to redundant programs
                //and Add() is unnecessary in that case
                for (int i = 1; i < output; i++) {
                    possible.Add(i);
                }
                //If output is 0, then Add function is not required, the same work can be done by
                //Element() function
                if (output == 0)
                    return null;
                result[inputState] = possible.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }

        //Value of op2 depends upon value of op1 and output of Add(), hence conditional witness
        //function is used.
        [WitnessFunction(nameof(Semantics.Add),1, DependsOnParameters = new[] { 0 })]
        public ExampleSpec Witnessaddop2(GrammarRule rule, ExampleSpec spec,ExampleSpec op1spec)
        {
            var result = new Dictionary<State, object>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var temp = (int?) op1spec.Examples[inputState];
                //value of op2 when output and op1 are given : op2 = output-op1
                result[inputState] =  output-temp;
            }
            return new ExampleSpec(result);
        }

        
        [WitnessFunction(nameof(Semantics.Mul), 0)]
        public DisjunctiveExamplesSpec Witnessmulop1(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, IEnumerable<object>>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var possible = new List<int>();
                //Domain of op1 is restricted to factors of output in the interval [2,output]
                //1 is not included in op1 because it would only lead to redundant programs
                //and Mul() is unnecessary in that case
                for (int i = 2; i <= output; i++)
                {
                    if(output%i==0)
                        possible.Add(i);
                }
                //If output is 0, then Mul() function is not required, the same work can be done by
                //Element() function
                if (output == 0 || possible.Count == 0)
                    return null;
                result[inputState] = possible.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }

        //Value of op2 depends upon value of op1 and output of Mul(), hence conditional witness
        //function is used.
        [WitnessFunction(nameof(Semantics.Mul), 1, DependsOnParameters = new[] { 0 })]
        public ExampleSpec Witnessmulop2(GrammarRule rule, ExampleSpec spec, ExampleSpec op1spec)
        {
            var result = new Dictionary<State, object>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var temp = (int?)op1spec.Examples[inputState];
                //value of op2 when output and op1 are given: op2 = output/op1
                result[inputState] = output/temp;
            }
            return new ExampleSpec(result);
        }

        //Similar approach as the above two witness functions is used, with the domain of op1 
        //being the multiples of output
        [WitnessFunction(nameof(Semantics.Div), 0)]
        public DisjunctiveExamplesSpec Witnessdivop1(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, IEnumerable<object>>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var possible = new List<int?>();
                if (output == 0)
                    return null;
                else
                {
                    for (int i = 2; i <= 10000/output; i++)
                    {
                        possible.Add(output * i);
                    }
                }
                if (possible.Count == 0)
                    return null;
                result[inputState] = possible.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }
        
        [WitnessFunction(nameof(Semantics.Div), 1, DependsOnParameters = new[] { 0 })]
        public ExampleSpec Witnessdivop2(GrammarRule rule, ExampleSpec spec, ExampleSpec op1spec)
        {
            var result = new Dictionary<State, object>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as int?;
                var temp = (int?)op1spec.Examples[inputState];
                if (output == 0 || temp==output)
                    return null;
                result[inputState] = temp/output;
            }
            return new ExampleSpec(result);
        }
        
        //pos(position of element) can be obtained by finding the indexes of the output element
        //in the input list.
        [WitnessFunction(nameof(Semantics.Element), 1)]
        public DisjunctiveExamplesSpec WitnessElement(GrammarRule rule, DisjunctiveExamplesSpec spec)
        {
            var result = new Dictionary<State, IEnumerable<object>>();

            foreach (KeyValuePair<State, IEnumerable<object>> example in spec.DisjunctiveExamples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as List<int>;
                var possible = new List<int>();
                foreach (int val in example.Value)
                {
                    if (input.Contains(val))
                        possible.Add(input.IndexOf(val));
                }
                if (possible.Count == 0) return null;
                result[inputState] = possible.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }
    }
}