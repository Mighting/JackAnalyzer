using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JackAnalyzer
{
    class JackTokenizer
    {
        private static string[] keyWords;
        private static string symbols;
        private static string operations;
        private static string[] libraries;
        private string[] tokens;
        private string jackcode;
        private string tokenType;
        private string keyWord;
        private char symbol;
        private string identifier;
        private string stringVal;
        private int intVal;
        private int pointer;
        private bool first;
        private static int place = 0;


        public JackTokenizer(string[] file)
        {
            Fill();
            try
            {
                for (int i = 0; i < file.Length; i++)
                {

                    using (StreamReader sr = new StreamReader(file[i]))
                    {
                        jackcode = "";
                        while (!sr.EndOfStream)
                        {
                            string strLine = sr.ReadLine();
                            while (strLine.Equals("") || HasComments(strLine))
                            {
                                if (HasComments(strLine))
                                {
                                    strLine = removeComments(strLine);
                                }
                                if (strLine.Trim().Equals(""))
                                {
                                    if (sr.EndOfStream)
                                    {
                                        strLine = sr.ReadLine();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            jackcode += strLine.Trim();
                        }
                    }
                }

                while (jackcode.Length > 0)
                {
                    while (jackcode[0] == ' ')
                    {
                        jackcode = jackcode.Substring(1);
                    }

                    // Keyword
                    for (int i = 0; i < keyWords.Length; i++)
                    {
                        if (jackcode.StartsWith(keyWords[i].ToString() + " "))
                        {
                            string keyword = keyWords[i].ToString();
                            tokens[place] = keyword;
                            place++;
                            jackcode = jackcode.Substring(keyword.Length);
                        }
                    }

                    // Symbol
                    if (symbols.Contains(jackcode.Substring(0, 1)))
                    {
                        char symbol = jackcode[0];
                        tokens[place] = symbol.ToString();
                        place++;
                        jackcode = jackcode.Substring(1);
                    }
                    else if (char.IsDigit(jackcode[0]))
                    {
                        string value = jackcode.Substring(0, 1);
                        jackcode = jackcode.Substring(1);
                        while (char.IsDigit(jackcode[0]))
                        {
                            value += jackcode.Substring(0, 1);
                            jackcode = jackcode.Substring(1);
                        }
                        tokens[place] = value;
                        place++;
                    }

                    // string constant
                    else if (jackcode.Substring(0, 1).Equals("\""))
                    {
                        jackcode = jackcode.Substring(1);
                        string strString = "\"";
                        while (jackcode[0] != '\"')
                        {
                            strString += jackcode[0];
                            jackcode = jackcode.Substring(1);
                        }
                        strString = strString + "\"";
                        tokens[place] = strString;
                        place++;
                        jackcode = jackcode.Substring(1);
                    }

                    // identifier
                    else if (char.IsLetter(jackcode[0]) || (jackcode.Substring(0, 1).Equals("_")))
                    {
                        string strIdentifier = jackcode.Substring(0, 1);
                        jackcode = jackcode.Substring(1);
                        while (char.IsLetter(jackcode[0]) || (jackcode.Substring(0, 1).Equals("_")))
                        {
                            strIdentifier += jackcode.Substring(0, 1);
                            jackcode = jackcode.Substring(1);
                        }
                        tokens[place] = strIdentifier;
                        place++;
                    }

                    // start out with pointer at pos 0
                    first = true;
                    pointer = 0;
                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: " + e);
            }
        }

        private static void Fill()
        {
            keyWords[0] = ("class");
            keyWords[1] = ("constructor");
            keyWords[2] = ("function");
            keyWords[3] = ("method");
            keyWords[4] = ("field");
            keyWords[5] = ("static");
            keyWords[6] = ("var");
            keyWords[7] = ("int");
            keyWords[8] = ("char");
            keyWords[9] = ("boolean");
            keyWords[10] = ("void");
            keyWords[11] = ("true");
            keyWords[12] = ("false");
            keyWords[13] = ("null");
            keyWords[14] = ("this");
            keyWords[15] = ("do");
            keyWords[16] = ("if");
            keyWords[17] = ("else");
            keyWords[18] = ("while");
            keyWords[19] = ("return");
            keyWords[20] = ("let");
            operations = "+-*/&|<>=";
            symbols = "{}()[].,;+-*/&|<>=-~";
            libraries[0] = ("Array");
            libraries[1] = ("Math");
            libraries[2] = ("String");
            libraries[3] = ("Array");
            libraries[4] = ("Output");
            libraries[5] = ("Screen");
            libraries[6] = ("Keyboard");
            libraries[7] = ("Memory");
            libraries[8] = ("Sys");
            libraries[9] = ("Square");
            libraries[10] = ("SquareGame");

        }

        public bool HasMoreTokens()
        {
            bool hasMore = false;
            if (pointer < tokens.Length - 1)
            {
                hasMore = true;
            }
            return hasMore;
        }

        public void Advance()
        {
            if (HasMoreTokens())
            {
                if (!first)
                {
                    pointer++;
                }
                else if (first)
                {
                    first = false;
                }

                string currentItem = tokens[pointer];
                if (keyWords.Contains(currentItem))
                {
                    tokenType = "KEYWORD";
                    keyWord = currentItem;
                }
                else if (symbols.Contains(currentItem))
                {
                    symbol = currentItem[0];
                    tokenType = "SYMBOL";
                }
                else if (char.IsDigit(currentItem[0]))
                {
                    intVal = int.Parse(currentItem);
                    tokenType = "INT_CONST";
                }
                else if (currentItem.Substring(0, 1).Equals("\""))
                {
                    tokenType = "STRING_CONST";
                    stringVal = currentItem.Substring(1, currentItem.Length - 1);
                }
                else if (char.IsLetter(currentItem[0]) || (currentItem[0] == '_'))
                {
                    tokenType = "IDENTIFIER";
                    identifier = currentItem;
                }

            }
            else
            {
                return;
            }
        }

        public void DecrementPointer()
        {
            if (pointer > 0)
            {
                pointer--;
            }
        }

        private bool HasComments(string strLine)
        {
            bool hasComments = false;
            if (strLine.Contains("//") || strLine.Contains("/*") || strLine.Trim().StartsWith("*"))
            {
                hasComments = true;
            }
            return hasComments;
        }
        private string removeComments(string strLine)
        {
            string strNoComments = strLine;
            if (HasComments(strLine))
            {
                int offSet;
                if (strLine.Trim().StartsWith("*"))
                {
                    offSet = strLine.IndexOf("*");
                }
                else if (strLine.Contains("/*"))
                {
                    offSet = strLine.IndexOf("/*");
                }
                else
                {
                    offSet = strLine.IndexOf("//");
                }
                strNoComments = strLine.Substring(0, offSet).Trim();

            }
            return strNoComments;
        }

        public string TokenType()
        {
            return tokenType;
        }

        // returns the keyword which is the current token, should be called only when tokenType() is keyword
        public string KeyWord()
        {
            return keyWord;
        }

        // returns character which is current token, should be called only when tokenType() is symbol
        public char Symbol()
        {
            return symbol;
        }

        // returns identifier which is the current token - should be called only when tokenType() is identifier
        public string Identifier()
        {
            return identifier;
        }

        // returns integer value of the current token - should be called only when tokenType() is INT_CONST
        public int IntVal()
        {
            return intVal;
        }

        // returns string value of current token without double quotes, should be called only when tokenType() is string_const
        public string StringVal()
        {
            return stringVal;
        }

        public bool IsOperation()
        {
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i] == symbol)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
