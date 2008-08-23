using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Ilsrep.PollApplication.Helpers
{
    public class InputHelper
    {
        /// <summary>
        /// Helper method to get answers on questions, where answer lies in a specific range
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="bottomLimit">bottom limit of the range</param>
        /// <param name="topLimit">top limit of the range</param>
        /// <returns>Answer from user</returns>
        public static int AskQuestion( string question, int bottomLimit, int topLimit )
        {
            int inputLine = 0;

            while ( true )
            {
                Console.Write( question );

                try
                {
                    inputLine = Convert.ToInt32( Console.ReadLine() );

                    if ( inputLine >= bottomLimit && inputLine <= topLimit )
                        return inputLine;
                    else
                        throw new Exception();
                }
                catch ( Exception )
                {
                    Console.WriteLine( "Wrong input!" );
                }
            }
        }

        /// <summary>
        /// Helper method to get answers on questions, with possibility of choosing what user can answer
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="allowedAnswers">Allowed answers that user can input. Set to null if any</param>
        /// <returns>Answer from user</returns>
        public static string AskQuestion( string question, string[] allowedAnswers )
        {
            string inputLine = String.Empty;

            while ( true )
            {
                Console.Write( question );
                inputLine = Console.ReadLine();

                if ( inputLine != String.Empty )
                {
                    if ( allowedAnswers == null || allowedAnswers.Length == 0 )
                        return inputLine;

                    foreach ( string allowedAnswer in allowedAnswers )
                        if ( inputLine == allowedAnswer )
                            return inputLine;
                }

                Console.WriteLine( "Wrong input!" );
            }
        }

        /// <summary>
        /// Helper method to get answers on questions with default answer, with possibility of choosing what user can answer
        /// Empty answer is not allowed
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="defaultAnswer">Default answer that gets filled in</param>
        /// <param name="allowedAnswers">Allowed answers that user can input. Set to null if any</param>
        /// <returns>Answer from user</returns>
        public static string AskQuestion( string question, string defaultAnswer, string[] allowedAnswers )
        {
            while ( true )
            {
                string inputLine = defaultAnswer;
                Console.Write( question + defaultAnswer );

                for ( ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey(true) )
                {
                    //rollback the cursor and write a space so it looks backspaced to the user
                    if ( cki.Key == ConsoleKey.Backspace )
                    {
                        if (inputLine.Length == 0)
                            continue;

                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write( " " );
                        Console.SetCursorPosition( Console.CursorLeft - 1, Console.CursorTop );
                        inputLine = inputLine.Substring( 0, inputLine.Length - 1 );
                    }
                    else
                    {
                        inputLine += cki.KeyChar;
                        Console.Write(cki.KeyChar);
                    }
                }

                Console.WriteLine();
                if ( inputLine != String.Empty )
                {
                    if ( allowedAnswers == null || allowedAnswers.Length == 0 )
                        return inputLine;

                    foreach ( string allowedAnswer in allowedAnswers )
                        if ( inputLine == allowedAnswer )
                            return inputLine;
                }

                Console.WriteLine( "Wrong input!" );
            }
        }

        /// <summary>
        /// Converts string to boolean
        /// </summary>
        /// <param name="boolString">string containing either "y" or "n"</param>
        /// <returns>true - when string equals to "y", false - otherwise</returns>
        public static bool ToBoolean( string boolString )
        {
            if ( boolString == "y" )
                return true;
            else
                return false;
        }

        /// <summary>
        /// Convery boolean to string
        /// </summary>
        /// <param name="boolValue">boolean value</param>
        /// <returns>string "y" if boolValue is true, else "n"</returns>
        public static string ToString( bool boolValue )
        {
            if ( boolValue )
                return "y";
            else
                return "n";
        }
    }
}