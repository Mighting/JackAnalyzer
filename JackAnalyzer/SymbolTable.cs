using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackAnalyzer
{
    class SymbolTable
    {
        private Dictionary<string, Symbol> classTable;
        private Dictionary<string, Symbol> methodTable;
        private Dictionary<string, int> indices;

        public SymbolTable()
        {
            classTable = new Dictionary<string, Symbol>();
            methodTable = new Dictionary<string, Symbol>();
            indices = new Dictionary<string, int>();
            indices.Add("static", 0);
            indices.Add("field", 0);
            indices.Add("argument", 0);
            indices.Add("var", 0);
        }

        public void startSubroutine()
        {
            methodTable.Clear();
            //indices.Add("argument", 0);
            //indices.Add("var", 0);
        }

        public void define(string strName, string strType, string strKind)
        {
            int index = indices[strKind];
            Symbol symbol = new Symbol(strType, strKind, index);
            index++;
            indices.Add(strKind, index);

            if (strKind.Equals("argument") || strKind.Equals("var"))
            {
                methodTable.Add(strName, symbol);
            }
            else if (strKind.Equals("static") || strKind.Equals("field"))
            {
                classTable.Add(strName, symbol);
            }

        }

        public int varCount(string strKind)
        {
            return indices[strKind];
        }

        public string KindOf(string strName)
        {
            string kind;
            if (methodTable.ContainsKey(strName))
            {
                kind = methodTable[strName].Kind;
            }
            else if (classTable.ContainsKey(strName))
            {
                kind = classTable[strName].Kind;
            }
            else
            {
                kind = "none";
            }
            return kind;
        }

        public string TypeOf(string strName)
        {
            string type;

            if (methodTable.ContainsKey(strName))
            {
                type = methodTable[strName].Type;
            }
            else if (classTable.ContainsKey(strName))
            {
                type = classTable[strName].Type;
            }
            else
            {
                type = "";
            }
            return type;
        }

        public int IndexOf(string strName)
        {
            Symbol symbol = null;
            int index;

            if (methodTable.ContainsKey(strName))
            {
                symbol = methodTable[strName];
            }
            else if (classTable.ContainsKey(strName))
            {
                symbol = classTable[strName];
            }

            if (symbol == null)
            {
                index = -1;
            }
            else
            {
                index = symbol.Number;
            }
            return index;
        }
    }
}
