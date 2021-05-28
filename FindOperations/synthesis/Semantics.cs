using System.Collections.Generic;

namespace FindOperations
{
    
    public static class Semantics
    {
        public static int Add(int op1, int op2)
        {
            return op1 + op2;
        }
        
        public static int Mul(int op1, int op2)
        {
            return op1 * op2;
        }
        
        public static int? Div(int op1, int op2)
        {
            if (op2 == 0)
                return null;
            if (op1 % op2 != 0)
                return null;
            return op1 / op2;
            
        }
        
        public static int? Element(List<int> lst, int pos)
        {
            if (pos < 0 || pos >= lst.Count)
                return null;
            return lst[pos];
        }
    }
}