using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JackAnalyzer
{
    class VMWriter
    {
        public StreamWriter sw;

        public VMWriter(string path)
        {
            try
            {
                sw = File.CreateText(path);

            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: No File Found: " + e);
            }
        }

        public void WritePush(string strSegment, int index)
        {
            if (strSegment.Equals("var"))
            {
                strSegment = "local";
            }
            if (strSegment.Equals("field"))
            {
                strSegment = "this";
            }
            try
            {
                sw.Write("push" + strSegment + " " + index + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to Write Push: " + e);
            }
        }

        public void WritePop(string strSegment, int index)
        {
            if (strSegment.Equals("var"))
            {
                strSegment = "local";
            }
            if (strSegment.Equals("field"))
            {
                strSegment = "this";
            }
            try
            {
                sw.Write("pop" + strSegment + " " + index + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to Write pop: " + e);
            }
        }

        public void WriteArithmetic(string strCommand)
        {
            try
            {
                sw.Write(strCommand + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write arithmetic: " + e);
            }

        }

        public void WriteLabel(string strLabel)
        {
            try
            {
                sw.Write("label" + strLabel + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write label: " + e);
            }
        }

        public void WriteGoto(string strLabel)
        {
            try
            {
                sw.Write("goto " + strLabel + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write goto: " + e);
            }

        }

        public void WriteIf(string strLabel)
        {
            try
            {
                sw.Write("if-goto " + strLabel + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write if-goto: " + e);
            }

        }

        public void WriteCall(string strName, int nArgs)
        {
            try
            {
                sw.Write("call " + strName + " " + nArgs + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write Call: " + e);
            }

        }

        public void WriteFunction(string strName, int nLocals)
        {
            try
            {
                sw.Write("function " + strName + " " + nLocals + "\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write Function: " + e);
            }

        }

        public void WriteReturn()
        {
            try
            {
                sw.Write("return\n");
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to write return: " + e);
            }

        }

        public void Close()
        {
            try
            {
                sw.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("VMWriter: Failed to close streamwriter: " + e);
            }

        }
    }
}
