using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robocopy_gui
{
    internal class TextFileHandling
    {        ///<summary>
             ///Returns the content of a file.
             /// </summary>
             /// <param name="sFilename">Path to file</param>
        public static string ReadFile(string sFilename)
        {
            string sContent = "";
            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            return sContent;
        }

        ///<summary>
        ///Writes the given text into a file.
        /// </summary>
        /// <param name="sFilename">Path to file</param>
        /// <param name="sLines">Text to write</param>
        public static void WriteFile(String sFilename, String sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename);
            myFile.Write(sLines);
            myFile.Close();
        }

        ///<summary>
        ///Appends the given text to the end of a file.
        ///</summary>
        ///<param name="sFilename">Path to file</param>
        ///<param name="sLines">Text to append</param>
        public static void Append(string sFilename, string sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename, true);
            myFile.Write(sLines);
            myFile.Close();
        }

        ///<summary>
        ///Returns the content of a given line.
        ///</summary>
        ///<param name="sFilename">Path to file</param>
        ///<param name="iLine">Line number</param>
        public static string ReadLine(String sFilename, int iLine)
        {
            var sContent = "";
            float fRow = 0;
            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                while (!myFile.EndOfStream && fRow < iLine)
                {
                    fRow++;
                    sContent = myFile.ReadLine();
                    if(sContent == null)
                    {
                        sContent = "";
                    }
                }
                myFile.Close();
                if (fRow < iLine)
                    sContent = "";
            }
            return sContent;
        }

        /// <summary>
        ///Writes the given text to a specific line.
        ///</summary>
        ///<param name="sFilename">Path to file</param>
        ///<param name="iLine">Line number</param>
        ///<param name="sLines">Text to write</param>
        ///<param name="bReplace">Replace text in line (t) or insert it (f)?</param>
        public static void WriteLine(String sFilename, int iLine, string sLines, bool bReplace)
        {
            string sContent = "";
            string[] delimiterstring = { "\r\n" };
            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            string[] sCols = sContent.Split(delimiterstring, StringSplitOptions.None);
            if (sCols.Length >= iLine)
            {
                if (!bReplace)
                    sCols[iLine - 1] = sLines + "\r\n" + sCols[iLine - 1];
                else
                    sCols[iLine - 1] = sLines;

                sContent = "";
                for (int x = 0; x < sCols.Length - 1; x++)
                {
                    sContent += sCols[x] + "\r\n";
                }
                sContent += sCols[sCols.Length - 1];
            }
            else
            {
                for (int x = 0; x < iLine - sCols.Length; x++)
                    sContent += "\r\n";
                sContent += sLines;
            }
            StreamWriter mySaveFile = new StreamWriter(sFilename);
            mySaveFile.Write(sContent);
            mySaveFile.Close();
        }

        ///<summary>
        ///Returns the number of lines in a file.
        /// </summary>
        /// <param name="Path">Path to file</param>
        public static int LineCount(string Path)
        {
            var lineCount = 0;
            using (var reader = File.OpenText(@Path))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }
            return lineCount;
        }
    }
}
