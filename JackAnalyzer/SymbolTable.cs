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
            indices.Add("static",0);
            indices.Add("field", 0);
            indices.Add("argument", 0);
            indices.Add("var", 0);
        }

        public void startSubroutine()
        {
            methodTable.Clear();
            indices.Add("argument",0);
            indices.Add("var", 0);
        }

        public void define(string strName, string strType, string strKind)
        {
            int index = indices[strKind];
        }
    }
}
