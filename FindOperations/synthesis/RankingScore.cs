using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Features;

namespace FindOperations
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score")
        {
        }

        protected override double GetFeatureValueForVariable(VariableNode variable)
        {
            return 0;
        }

        //Ranking has been defined in such a way, that same length programs
        //(programs having same number of operations) have the same score, and the score 
        //decreases with increasing length(This is ensured by the score of Element operation)
        [FeatureCalculator(nameof(Semantics.Add),Method =CalculationMethod.FromChildrenFeatureValues)]
        public static double Add(double op1,double op2)
        {
            return op1*op2;
        }

        [FeatureCalculator(nameof(Semantics.Mul), Method = CalculationMethod.FromChildrenFeatureValues)]
        public static double Mul(double op1, double op2)
        {
            return op1*op2;
        }
        
        [FeatureCalculator("Div", Method = CalculationMethod.FromChildrenFeatureValues)]
        public static double Div(double op1, double op2)
        {
            return op1 * op2;
        }

        //Uncomment this section if you want to rank the programs according to position of 
        //elements used in the program with lower indexed elements having higher score
        /*
        [FeatureCalculator(nameof(Semantics.Element))]
        public static double Element(double v, double pos)
        {
            return 1.0/(2+pos);
        }
        */

        //Value has been taken less than 1 so that succesive multiplication would lead to
        //decreasing score
        [FeatureCalculator(nameof(Semantics.Element))]
        public static double Element(double v, double pos)
        {
            return 0.9;
        }

        //This score of 'pos' has no effect in this ranking, but if the above commented portion of
        //Element() score is used, then this would matter.
        [FeatureCalculator("pos", Method = CalculationMethod.FromLiteral)]
        public static double Pos(int pos)
        {
            return pos;
        }
    }
}