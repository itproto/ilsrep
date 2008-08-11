using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Ilsrep.PollApplication.Helpers
{
    /// <summary>
    /// Helper that parses command line parameters
    /// </summary>
    public class CommandLineParametersHelper
    {
        /// <summary>
        /// Parse command line parameters from -var1 value1 -var2 value2 to NameValueCollection
        /// </summary>
        /// <param name="args">list of arguments in Array as passed to Main method ("-var1", "value1", "-var2", "value2")</param>
        /// <returns>NameValueCollection of arguments as NameValueCollection["var1"]="value1"</returns>
        public static NameValueCollection Parse(string[] args)
        {
            String key = String.Empty;
            NameValueCollection commandLineParameters = new NameValueCollection();

            foreach (String commandLineParameter in args)
            {
                if (key == String.Empty && commandLineParameter.Substring(0, 1) == "-")
                {
                    key = commandLineParameter.Substring(1);
                }
                else if (key != String.Empty)
                {
                    commandLineParameters.Add(key, commandLineParameter);
                    key = String.Empty;
                }
            }

            return commandLineParameters;
        }
    }
}
