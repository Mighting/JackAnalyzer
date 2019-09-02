using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackAnalyzer
{
    class Symbol
    {

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string kind;

        public string Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        private int number;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public Symbol(string strType, string strKind, int nNumber)
        {
            this.type = strType;
            this.kind = strKind;
            this.number = nNumber;
        }



    }
}
