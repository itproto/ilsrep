using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.Common
{
    public class StringWorkHelper
    {
        public static string WrapString(String oldString, int maxCharCount)
        {
            String[] words = oldString.Split(' ');
            String lineString = String.Empty;
            String newString = String.Empty;

            int index = 0;
            foreach (String word in words)
            {
                lineString += word;

                if (lineString.Length > maxCharCount)
                {
                    String line = lineString.Remove(lineString.Length - word.Length);
                    newString += line;
                    newString += Environment.NewLine;
                    lineString = word;
                }

                if (index == words.Count() - 1)
                {
                    newString += lineString;
                }
                else
                {
                    lineString += " ";
                }

                index++;
            }

            return newString;
        }
    }
}
