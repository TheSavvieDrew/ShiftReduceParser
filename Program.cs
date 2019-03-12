using System;
using System.Collections.Generic;

namespace ShiftReduceParser
{
    public class Lexer
    {
        /**
         * <summary>List of tokens created by the Tokenize method.</summary>
         */
        public List<string> Tokens = new List<string>();

        /**
         * <summary>Method that converts an expression to a list of tokens</summary>
         * <param name="expression">Expression to be converted to a list of its tokens.</param>
         */
        public void Tokenize(string expression)
        {
            int index = 0;
            int length = 0;
            bool fail = false;
            while (index < expression.Length && !fail)
            {
                if (Char.IsDigit(expression[index]))
                {
                    index++;
                    length++;
                    while (index < expression.Length)
                    {
                        if (Char.IsDigit(expression[index]))
                        {
                            index++;
                            length++;
                        }
                        else if (expression[index] == ' ' || IsOperator(expression[index]))
                        {
                            Tokens.Add("id");
                            Console.WriteLine("{0}: id", expression.Substring(index - length, length));
                            length = 0;
                            break;
                        }
                        else
                        {
                            Tokens.Clear();
                            fail = true;
                            break;
                        }
                    }
                    if (index == expression.Length)
                    {
                        Tokens.Add("id");
                        Console.WriteLine("{0}: id", expression.Substring(index - length, length));
                        length = 0;
                    }
                }
                else if (Char.IsLetter(expression[index]))
                {
                    index++;
                    length++;
                    while (index < expression.Length)
                    {
                        if (Char.IsLetterOrDigit(expression[index]))
                        {
                            index++;
                            length++;
                        }
                        else if (expression[index] == ' ' || IsOperator(expression[index]))
                        {
                            Tokens.Add("id");
                            Console.WriteLine("{0}: id", expression.Substring(index - length, length));
                            length = 0;
                            break;
                        }
                        else
                            break;
                    }
                    if (index == expression.Length)
                    {
                        Tokens.Add("id");
                        Console.WriteLine("{0}: id", expression.Substring(index - length, length));
                        length = 0;
                    }
                }
                else if (expression[index] == ' ')
                    index++;
                else if (IsOperator(expression[index]))
                {
                    Tokens.Add(expression[index].ToString());
                    Console.WriteLine("{0}: {1}", expression[index], expression[index]);
                    index++;
                }
                else
                {
                    Tokens.Clear();
                    fail = true;
                    break;
                }
            }
            if (fail)
            {
                Console.WriteLine("Error: Lexer Detected Invalid Input ({0}). Process Terminated.", expression.Substring(index - length, length + 1));
                Console.Write("Press any key to exit...");
                Console.ReadLine();
            }
            else
            {
                Tokens.Add("$");
            }   
        }

        /**
         * <summary>Method that tests whether the character given is an operator.</summary>
         * <param name="lexeme">The character to be checked if its an operator.</param>
         * <returns>Returns a boolean.</returns>
         */
        private bool IsOperator(char lexeme)
        {
            if (lexeme == '=' || lexeme == '+' || lexeme == '*' || lexeme == '(' || lexeme == ')')
                return true;
            else
                return false;
        }
    }

    public class ShiftReduceParser
    {
        /**
         * <summary>2D array of shift and reduce actions based on the state rows and action columns.
         * Positive numbers mean shift by that amount, negative numbers mean reduce by that rule, and 
         * 1000 means accept.</summary>
         */
        private readonly int[,] stateAction = new int[12, 6]
        {
            { 5, 0, 0, 4, 0, 0 },
            { 0, 6, 0, 0, 0, 1000 },
            { 0, -2, 7, 0, -2, -2 },
            { 0, -4, -4, 0, -4, -4 },
            { 5, 0, 0, 4, 0, 0 },
            { 0, -6, -6, 0, -6, -6 },
            { 5, 0, 0, 4, 0, 0 },
            { 5, 0, 0, 4, 0, 0 },
            { 0, 6, 0, 0, 11 ,0 },
            { 0, -1, 7, 0, -1, -1 },
            { 0, -3, -3, 0, -3, -3 },
            { 0, -5, -5, 0, -5, -5 }
        };

        /**
         * <summary>2D array of states based on the state rows and goTo letter columns; -1 is an error.</summary>
         */
        private readonly int[,] goTo = new int[12, 3]
        {
            { 1, 2, 3 },
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
            { 8, 2, 3 },
            { -1, -1, -1 },
            { -1, 9, 3 },
            { -1, -1, 10 },
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
        };

        /**
         * <summary>Stack that contains the lists of states and nonterminals.</summary>
         */
        private Stack<List<string>> mainStack = new Stack<List<string>>();

        /**
         * <summary>Main function of the ShiftReduceParser class which just starts the process.</summary>
         * <param name="tokens">List of tokens to parse over.</param>
         */
        public void Start(List<string> tokens)
        {
            mainStack.Push(new List<string> { "0" });
            int value = stateAction[0, getIndex(tokens[0])];
            List<string> temp = new List<string>();
            string parsingSteps = "";
            bool fail = false;
            while (value != 1000 && fail == false)
            {
                switch (Math.Sign(value))
                {
                    case 1:
                        parsingSteps += "S" + value.ToString() + "\n";
                        temp.AddRange(mainStack.Peek());
                        temp.Add(tokens[0]);
                        temp.Add(value.ToString());
                        mainStack.Push(temp);
                        tokens = tokens.GetRange(1, tokens.Count - 1);
                        value = stateAction[value, getIndex(tokens[0])];
                        temp = new List<string>();
                        break;
                    case -1:
                        string letter = "";
                        switch (value)
                        {
                            case -1: case -2:
                                letter = "E";
                                break;
                            case -3: case -4:
                                letter = "T";
                                break;
                            case -5: case -6:
                                letter = "F";
                                break;
                        }
                        switch (value)
                        {
                            case -1: case -3: case -5:
                                parsingSteps += "R" + Math.Abs(value).ToString() + "\n";
                                temp.AddRange(mainStack.Peek());
                                temp = temp.GetRange(0, temp.Count - 6);
                                temp.Add(letter);
                                value = goTo[int.Parse(temp[temp.Count - 2]), Math.Abs(value) / 2];
                                temp.Add(value.ToString());
                                mainStack.Push(temp);
                                value = stateAction[value, getIndex(tokens[0])];
                                temp = new List<string>();
                                break;
                            case -2: case -4: case -6:
                                parsingSteps += "R" + Math.Abs(value).ToString() + "\n";
                                temp.AddRange(mainStack.Peek());
                                temp[temp.Count - 2] = letter;
                                value = goTo[int.Parse(temp[temp.Count - 3]), Math.Abs(value) / 2 - 1];
                                temp[temp.Count - 1] = value.ToString();
                                mainStack.Push(temp);
                                value = stateAction[value, getIndex(tokens[0])];
                                temp = new List<string>();
                                break;
                        }  
                        break;
                    default:
                        fail = true;
                        Console.Write("Error: Parser detected invalid input {0}.", tokens[0]);
                        break;
                }
            }
            if (!fail)
            {
                parsingSteps += "ACCEPT";
                Console.WriteLine(parsingSteps);
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Stack");
                Console.WriteLine("----------------------------------");
                List<string>[] stackPrint = mainStack.ToArray();
                for (int i = stackPrint.Length - 1; i >= 0; i--)
                {
                    Console.WriteLine(String.Join("", stackPrint[i]));
                }
            }
            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }

        /**
         * <summary>Finds the index for the stateAction array based on the nonterminal given.</summary>
         * <param name="value">String to find the cooresponding index.</param>
         */
        private int getIndex(string value)
        {
            switch (value)
            {
                case "id":
                    return 0;
                case "+":
                    return 1;
                case "*":
                    return 2;
                case "(":
                    return 3;
                case ")":
                    return 4;
                case "$":
                    return 5;
                default:
                    return -1;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter your expression:");
            string expression = Console.ReadLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Calling Lexer:");
            Console.WriteLine("----------------------------------");
            Lexer lex = new Lexer();
            lex.Tokenize(expression);
            if (lex.Tokens.Count != 0)
            {
                ShiftReduceParser SRP = new ShiftReduceParser();
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Parsing Steps");
                Console.WriteLine("----------------------------------");
                SRP.Start(lex.Tokens);
            }
        }
    }
}
