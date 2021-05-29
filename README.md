# Microsoft_Prose-Find_List_programs
Given a list of positive integers and a desired output, the given code generates all the programs that can give the desired output by operating on the given list according to the DSL specified below. Most of the code is accompanied with explanations and justifications through comments. However, the important points and overview of the code is as follows:

## Details of DSL
**Input** : A list of positive integers. For ex- {1,2,3,4}

**Output** : An integer

**Operations allowed**

 1. Add(int operand1, int operand2) : returns op1+op2  
 2. Multiply(int operand1, int operand2) : returns op1*op2  
 3. Divide(int operand1, int operand2): returns op1/op2  
 4. Element(pos) : returns the element of the list at index pos

## File Details

 - FindOps.grammar : Defines the grammar of the DSL (Located in FindOperations/synthesis/grammar/) 
 - Semantics.cs : Contains the semantical definitions of all the functions in the DSL. (Located in FindOperations/synthesis/)
 - WitnessFunctions.cs : Contains the declaration of all the witness functions for each of the parameters of the functions. (Located in FindOperations/synthesis/)
 - RankingScore.cs : Defines the score features. (Located in FindOperations/synthesis/)
 - Program.cs : Can be run for interaction with the user. (Located in FindOperations/synthesis/)
 - FindOpsTest.cs : Contains all the testcases which were passed thus verifying the accuracy of the code. (Located in FindOperations.Tests/) .


## Grammar
Grammar is defined in a recursive manner.

## Semantics
For division, null is returned when operand2 == 0.

## Witness Functions
In case of Add(operand1, operand2), Multiply(operand1, operand2) and Divide(operand1, operand2), first the witness functions for operand1 for each of the operations are defined seperately. For operand2 of each function, Conditional witness functions are generated for each function, where the output of operand2 depends upon the output given in the ExampleSpec and operand1.
In case of Element(List<int>, int pos), witness function for parameter pos which generates the constraints on pos by element matching.

It is made sure that Add() does not return any of it's operand as 0, Multiply() does not return any of it's operand as 0 or 1 and Division() doesn't return  operand1 as the same as output or operand2 as 1. This is because it only leads to redundancy and nothing useful.

## Ranking
The score features of various functions are defined in such a way that, **the smallest length programs(having minimum number of operations) are given the highest score**. As the program length(number of operations present) increases, the score and hence the rank of the program also decreases.
The score is calculated by the method of **Calculation from recursive values**. For example the code for calculating the score of operation add is as follows:

    [FeatureCalculator(nameof(Semantics.Add),Method =CalculationMethod.FromChildrenFeatureValues)]
    public static double Add(double op1,double op2)
    {
	    return op1*op2;
    }
And the function Element returns a value less than 1. This ensures that on successive multiplication( longer the program, the more number of times the succesive multiplication of values less than 1), the score of the program keeps on decreasing. Hence shorter programs give out the highest score and hence are ranked the highest.

**Note:**
**The elements in the array should not be greater than 1000**. 
For changing the upper limit, user would have to change the upper limit of for loop(in the github code, it's set as 1000) in the Div() witness function of operand1{Function name:  Witnessdivop1(GrammarRule  rule, ExampleSpec  spec) } in file WitnessFunctions.cs


                if (output == 0)
                    return null;
                else
                {
                    for (int i = 2; i <= 1000/output; i++)
                    {
                        possible.Add(output * i);
                    }
                }


As the upper limit increases, the domain of operands also increase in the witness function and this affects speed of program synthesis. This can vary according to computation power of individual pc. My pc was able to handle the upper limit till 1000000. Upon running with the upper limit as 1000, the programs were generated in 1 second. And upon running with upper limit as 1000000 it took 10 seconds to generate the programs. Hence the user should change the upper limit in accordance with speed and upper limit requirements. For the purpose of testing quickly against different input-outputs, I have kept the value 1000.


## Testing and Execution

The project was created in Visual Studio 2017.
For running the program and verifying it's results there are two options:

1. Open FindOpsTest.cs and remove the comment for whichever specification the user wants to generate the programs. Optionally, the user can provide his own set of input-output.

**![](https://lh6.googleusercontent.com/wWpbVL-Zw9fhyIPc0Y1sgC5DvYYtqebpcEra4x6nBI-2XtxdguHkgtyHIUCJUZz2AajUE-wQVoOfXlv1OSo6NynV6MX6ZCnqHIe-Rj5AkiZlyp9tUzdi3dgOqDYv9zAckOi5J9ES)**

This is a sample output of a test case. The list given is {10,12,14,20} and the desired output is 26. By using the four operations mentioned above, a set of programs is created to give out the desired output 26.
                      
**![](https://lh3.googleusercontent.com/szUFBb6YqunVuitX5FblhzgHil_pY56bmRu5dYsKCwVEJigjoONvJaMOcxYi5jHG4mh13qbEpVfR3brKLWJHHa0-UPoJSo3OzIpTBOz9mG0ipX1gyWriT7NzTeR09fBtf0LdePKI)**

In case program set can't be generated, an error is shown stating "No program was found for this specification".
                                                     
**![](https://lh5.googleusercontent.com/u0oKQFaGJKBk-MeTiEk-JVCeT7milGy7J2WE5IyPcEdA7hRakEMgLqYvKFjNWNDBQyr5GD3FStQQCYubCg6ppi_b2qX9YTOZ4jG8vyHSlz3OMFB-tCYP8tqu4gqZoqbWE2oxq17E)**


2. The user can run **Program.cs**

**![](https://lh6.googleusercontent.com/dmAAMJyjacyYaRw2VqCnqOPcqwzlwXaKwHHngGegSthHsuRuASBZdaRyPm2EocsICmdZ1WLjOAlA5l4mFU7jTmL71KHEjt4-GwmNou1AULdgeN4XE9-kJl_kvFEzj4ukuSZ_w5dF)**


On running FindOperation through the option given below, a console window is created.
                                                     
**![](https://lh5.googleusercontent.com/eT0x9DRrPBIuw9GLCHHVyzoF7LtKqzCf_qsWsqZFAfViwgn13iCLzWM4W2OBYI2RUWT8HRLJnYhH0rsjeYwhGVHp7IhWKz1PNNY8E3q5Ph7K15UO2X8uCwS-0ejzX-fSz43Hz3r2)**

The console window gives user the following options.

**![](https://lh6.googleusercontent.com/QqczIv7fCfwbCgEjbUkKIX4ges9sN5QudZR6DWJbH2Tqg2pXn0lyUCBEiQ1wqHLVUaO5OnB6Cf_YU2bLrhf2MnhvNN6yR_m_sBY9k6mLR7YCRuOzc2bEeo5vLqM5rrAQV9rtTc_T)**

On enetering the specifications in the proper format and pressing enter, the program set generated is shown in the output window. Max programs sets shown on the window are 50 when there are more than that.
                                                     
**![](https://lh5.googleusercontent.com/8o6Ywyc0mCHBt2gkQSTUoh65FRRVQetVhVYXMABXHRauRakw43tumTb9SD7Oe5Nilppg1nFJjHv3rsqRxVA_j5mIjXF80a4ujPRvoE4Ui0yPxOzmHssNhpm5ReCh1jH4_nk4C-Br)**


                                     
