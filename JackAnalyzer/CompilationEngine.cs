using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JackAnalyzer
{
    class CompilationEngine
    {
        private JackTokenizer jtoken;
        private SymbolTable symbolTable;
        private VMWriter vmWriter;
        private string strClassName = "";
        private string strSubRoutineName = "";

        private int labelIndex;

        public CompilationEngine(string[] file, string path)
        {
            jtoken = new JackTokenizer(file);
            symbolTable = new SymbolTable();
            vmWriter = new VMWriter(path);
            labelIndex = 0;
        }

        public void compileClass()
        {
            jtoken.Advance();
            jtoken.Advance();

            strClassName = jtoken.Identifier();
            jtoken.Advance();
            CompileClassVarDec();
            CompileSubRoutine();
            vmWriter.Close();
        }

        public void CompileClassVarDec()
        {
            jtoken.Advance();
            while (jtoken.KeyWord().Equals("static") || jtoken.KeyWord().Equals("field"))
            {
                string strKind;
                string type;
                if (jtoken.KeyWord().Equals("static"))
                {
                    strKind = "static";
                }
                else
                {
                    strKind = "field";
                }
                jtoken.Advance();

                if (jtoken.TokenType().Equals("IDENTIFIER"))
                {
                    type = jtoken.KeyWord();
                }
                else
                {
                    type = jtoken.KeyWord();
                }

                jtoken.Advance();
                symbolTable.define(jtoken.Identifier(),type , strKind);
                jtoken.Advance();

                while (jtoken.Symbol() == ',')
                {
                    jtoken.Advance();
                    symbolTable.define(jtoken.Identifier(), type, strKind);
                    jtoken.Advance();
                }

                jtoken.Advance();
            }

            if (jtoken.KeyWord().Equals("function") || (jtoken.KeyWord().Equals("method")) || (jtoken.KeyWord().Equals("constructor")))
            {
                jtoken.DecrementPointer();
                return;
            }
        }

        public void CompileSubRoutine()
        {
            jtoken.Advance();

            if (jtoken.Symbol() == ')' && jtoken.TokenType().Equals("SYMBOL"))
            {
                return;
            }
            string strKeyword = "";

            if (jtoken.KeyWord().Equals("function") || jtoken.KeyWord().Equals("method") || jtoken.KeyWord().Equals("constructor"))
            {
                strKeyword = jtoken.KeyWord();
                // new subroutine - reset symbol table
                symbolTable.startSubroutine();
                if (jtoken.KeyWord().Equals("method"))
                {
                    symbolTable.define("this", strClassName, "argument");

                }
                jtoken.Advance();
            }

            string strType = "";

            if (jtoken.TokenType().Equals("KEYWORD") && jtoken.KeyWord().Equals("void"))
            {
                strType = "void";
                jtoken.Advance();
            }
            else if (jtoken.TokenType().Equals("KEYWORD") && (jtoken.KeyWord().Equals("int") || jtoken.KeyWord().Equals("boolean") || jtoken.KeyWord().Equals("char")))
            {
                strType = jtoken.KeyWord();
                jtoken.Advance();
            }
            else
            {
                strType = jtoken.Identifier();
                jtoken.Advance();
            }

            if (jtoken.TokenType().Equals("IDENTIFIER"))
            {
                strSubRoutineName = jtoken.Identifier();
                jtoken.Advance();
            }

            if (jtoken.Symbol() == '(')
            {
                CompileParameterList();
            }

            jtoken.Advance();

            if (jtoken.Symbol() == '{')
            {
                jtoken.Advance();
            }

            while (jtoken.KeyWord().Equals("var") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                jtoken.DecrementPointer();
                CompileVarDec();
            }

            string strFunction = "";

            if (strClassName.Length != 0 && strSubRoutineName.Length != 0)
            {
                strFunction += strClassName + "." + strSubRoutineName;
            }
            vmWriter.WriteFunction(strFunction,symbolTable.varCount("var"));

            if (strKeyword.Equals("method"))
            {
                vmWriter.WritePush("argument", 0);
                vmWriter.WritePop("pointer", 0);
            }
            else if (strKeyword.Equals("constructor"))
            {
                vmWriter.WritePush("constant",symbolTable.varCount("field"));
                vmWriter.WriteCall("Memory.alloc",1);
                vmWriter.WritePop("pointer", 0);
            }

            CompileStatements();

            CompileSubRoutine();
        }

        public void CompileParameterList()
        {
            jtoken.Advance();
            string type = "";
            string name = "";
            bool hasParam = false;

            while (!(jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == ')'))
            {
                if (jtoken.TokenType().Equals("KEYWORD"))
                {
                    hasParam = true;
                    type = jtoken.KeyWord();
                }
                else if (jtoken.TokenType().Equals("IDENTIFIER"))
                {
                    type = jtoken.Identifier();
                }
                jtoken.Advance();

                if (jtoken.TokenType().Equals("IDENTIFIER"))
                {
                    name = jtoken.Identifier();
                }
                jtoken.Advance();

                if (jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == ',')
                {
                    symbolTable.define(name,type,"argument");
                    jtoken.Advance();
                }
            }

            if (hasParam)
            {
                symbolTable.define(name,type,"argument");
            }
        }

        public void CompileVarDec()
        {
            jtoken.Advance();
            string type = "";
            string name = "";
            if (jtoken.KeyWord().Equals("var") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                jtoken.Advance();
            }

            if (jtoken.TokenType().Equals("IDENTIFIER"))
            {
                type = jtoken.Identifier();
                jtoken.Advance();
            }
            else if (jtoken.TokenType().Equals("KEYWORD"))
            {
                type = jtoken.KeyWord();
                jtoken.Advance();
            }

            if (jtoken.TokenType().Equals("IDENTIFIER"))
            {
                name = jtoken.Identifier();
                jtoken.Advance();
            }
            symbolTable.define(name,type,"var");

            while ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == ','))
            {
                jtoken.Advance();
                name = jtoken.Identifier();
                symbolTable.define(name,type,"var");

                jtoken.Advance();
            }

            if ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == ';'))
            {
                jtoken.Advance();
            }
        }


        public void CompileStatements()
        {
            if (jtoken.Symbol() == ')' && (jtoken.TokenType().Equals("KEYWORD")))
            {
                return;
            }
            else if (jtoken.KeyWord().Equals("do") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                CompileDo();
            }
            else if (jtoken.KeyWord().Equals("let") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                CompileLet();
            }
            else if (jtoken.KeyWord().Equals("if") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                CompileIf();
            }
            else if (jtoken.KeyWord().Equals("while") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                CompileWhile();
            }
            else if (jtoken.KeyWord().Equals("return") && (jtoken.TokenType().Equals("KEYWORD")))
            {
                CompileReturn();
            }
            jtoken.Advance();
            CompileStatements();
        }

        public void CompileDo()
        {
            if (jtoken.KeyWord().Equals("do"))
            {
                //Ehhh what?
            }

            CompileCall();
            jtoken.Advance();
            vmWriter.WritePop("temp", 0);
        }

        public void CompileCall()
        {
            jtoken.Advance();
            string first = jtoken.Identifier();
            int arguments = 0;
            jtoken.Advance();
            if ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == '.'))
            {
                string objectName = first;

                jtoken.Advance();
                jtoken.Advance();
                first = jtoken.Identifier();
                string strType = symbolTable.TypeOf(objectName);
                if (strType.Equals(""))
                {
                    first = objectName + "." + first;
                }
                else
                {
                    arguments = 1;
                    vmWriter.WritePush(symbolTable.KindOf(objectName), symbolTable.IndexOf(objectName));
                    first = symbolTable.TypeOf(objectName) + "." + first;
                }

                arguments += CompileExpressionList();
                jtoken.Advance();
                vmWriter.WriteCall(first, arguments);
            }
            else if ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == '('))
            {
                vmWriter.WritePush("pointer", 0);

                arguments = CompileExpressionList();
                jtoken.Advance();
                vmWriter.WriteCall(strClassName + "." + first, arguments);
            }
        }


        public void CompileLet()
        {
            jtoken.Advance();
            string strVariableName = jtoken.Identifier();
            jtoken.Advance();
            bool array = false;
            if ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == '['))
            {
                array = true;
                vmWriter.WritePush(symbolTable.KindOf(strVariableName), symbolTable.IndexOf(strVariableName));
                CompileExpressionList();
                jtoken.Advance();
                if ((jtoken.TokenType().Equals("SYMBOL")) && (jtoken.Symbol() == ']'))
                {
                    vmWriter.WriteArithmetic("add");
                    jtoken.Advance();
                }
            }

            CompileExpressionList();
            jtoken.Advance();
            if (array)
            {
                vmWriter.WritePop("temp",0);
                vmWriter.WritePop("pointer",1);
                vmWriter.WritePush("temp",0);
                vmWriter.WritePop("that",0);
            }
            else
            {
                vmWriter.WritePop(symbolTable.KindOf(strVariableName),symbolTable.IndexOf(strVariableName));
            }
        }

        public void CompileWhile()
        {
            string secondLabel = "LABEL_" + labelIndex++;
            string firstLabel = "LABEL_" + labelIndex++;
            vmWriter.WriteLabel(firstLabel);

            jtoken.Advance();

            CompileExpression();

            jtoken.Advance();

            vmWriter.WriteArithmetic("not");
            vmWriter.WriteIf(secondLabel);
            jtoken.Advance();

            CompileStatements();

            vmWriter.WriteGoto(firstLabel);

            vmWriter.WriteLabel(secondLabel);
        }

        public void CompileReturn()
        {
            jtoken.Advance();
            if (!((jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == ';')))
            {
                jtoken.DecrementPointer();
                CompileExpression();
            }
            else if (jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == ';')
            {
                vmWriter.WritePush("constant", 0);
            }
            vmWriter.WriteReturn();
        }

        public void CompileIf()
        {
            string strLabelElse = "LABEL_" + labelIndex++;
            string strLabelEnd = "LABEL_" + labelIndex++;
            jtoken.Advance();

            CompileExpression();
            jtoken.Advance();

            vmWriter.WriteArithmetic("not");
            vmWriter.WriteIf(strLabelElse);
            jtoken.Advance();

            CompileStatements();

            vmWriter.WriteGoto(strLabelEnd);
            vmWriter.WriteLabel(strLabelElse);
            jtoken.Advance();

            if (jtoken.TokenType().Equals("KEYWORD") && jtoken.KeyWord().Equals("else"))
            {
                jtoken.Advance();
                jtoken.Advance();

                CompileStatements();
            }
            else
            {

                jtoken.DecrementPointer();
            }
            vmWriter.WriteLabel(strLabelEnd);
        }

        public void CompileExpression()
        {
            CompileTerm();
            while (true)
            {
                jtoken.Advance();
                if (jtoken.TokenType().Equals("SYMBOL") && jtoken.IsOperation())
                {
                    if (jtoken.Symbol() == '<')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("lt");
                    }
                    else if (jtoken.Symbol() == '>')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("gt");
                    }
                    else if (jtoken.Symbol() == '&')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("and");
                    }
                    else if (jtoken.Symbol() == '+')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("add");
                    }
                    else if (jtoken.Symbol() == '-')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("sub");
                    }
                    else if (jtoken.Symbol() == '*')
                    {
                        CompileTerm();
                        vmWriter.WriteCall("Math.multiply", 2);
                    }
                    else if (jtoken.Symbol() == '/')
                    {
                        CompileTerm();
                        vmWriter.WriteCall("Math.divide", 2);
                    }
                    else if (jtoken.Symbol() == '=')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("eq");
                    }
                    else if (jtoken.Symbol() == '|')
                    {
                        CompileTerm();
                        vmWriter.WriteArithmetic("or");
                    }

                }
                else
                {
                    jtoken.DecrementPointer();
                    break;
                }
            }
        }

        public void CompileTerm()
        {
            jtoken.Advance();
            if (jtoken.TokenType().Equals("IDENTIFIER"))
            {
                string prevIdentifier = jtoken.Identifier();
                jtoken.Advance();
                if (jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == '[')
                {
                    vmWriter.WritePush(symbolTable.KindOf(prevIdentifier), symbolTable.IndexOf(prevIdentifier));
                    CompileExpression();
                    jtoken.Advance();
                    vmWriter.WriteArithmetic("add");
                    vmWriter.WritePop("pointer", 1);
                    vmWriter.WritePush("that", 0);
                }
                else if (jtoken.TokenType().Equals("SYMBOL") && (jtoken.Symbol() == '(' || jtoken.Symbol() == '.'))
                {
                    jtoken.DecrementPointer();
                    jtoken.DecrementPointer();
                    CompileCall();

                }
                else
                {
                    jtoken.DecrementPointer();
                    vmWriter.WritePush(symbolTable.KindOf(prevIdentifier), symbolTable.IndexOf(prevIdentifier));
                }
            }
            else
            {
                if (jtoken.TokenType().Equals("INT_CONST"))
                {
                    vmWriter.WritePush("constant", jtoken.IntVal());

                }
                else if (jtoken.TokenType().Equals("STRING_CONST"))
                {
                    string strToken = jtoken.StringVal();
                    vmWriter.WritePush("constant", strToken.Length);
                    vmWriter.WriteCall("String.new", 1);
                    for (int i = 0; i < strToken.Length; i++)
                    {
                        vmWriter.WritePush("constant", strToken[i]);
                        vmWriter.WriteCall("String.appendChar", 2);
                    }
                }
                else if (jtoken.TokenType().Equals("KEYWORD") && jtoken.KeyWord().Equals("this"))
                {
                    vmWriter.WritePush("pointer", 0);
                }
                else if (jtoken.TokenType().Equals("KEYWORD") && (jtoken.KeyWord().Equals("null") || jtoken.KeyWord().Equals("false")))
                {
                    vmWriter.WritePush("constant", 0);

                }
                else if (jtoken.TokenType().Equals("KEYWORD") && jtoken.KeyWord().Equals("true"))
                {
                    vmWriter.WritePush("constant", 0);
                    vmWriter.WriteArithmetic("not");
                }

                else if (jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == '(')
                {
                    CompileExpression();
                    jtoken.Advance();
                }
                else if (jtoken.TokenType().Equals("SYMBOL") && (jtoken.Symbol() == '-' || jtoken.Symbol() == '~'))
                {
                    char symbol = jtoken.Symbol();
                    CompileTerm();
                    if (symbol == '-')
                    {
                        vmWriter.WriteArithmetic("neg");
                    }
                    else if (symbol == '~')
                    {
                        vmWriter.WriteArithmetic("not");
                    }
                }
            }

        }

        public int CompileExpressionList()
        {
            int arguments = 0;
            jtoken.Advance();
            if (jtoken.Symbol() == ')' && jtoken.TokenType().Equals("SYMBOL"))
            {
                jtoken.DecrementPointer();
            }
            else
            {
                arguments = 1;
                jtoken.DecrementPointer();
                CompileExpression();
            }
            while (true)
            {
                jtoken.Advance();
                if (jtoken.TokenType().Equals("SYMBOL") && jtoken.Symbol() == ',')
                {
                    CompileExpression();
                    arguments++;
                }
                else
                {
                    jtoken.DecrementPointer();
                    break;
                }
            }
            return arguments;

        }

    }
}
