using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Ilsrep.PollApplication.Helpers
{
    public class CommandLineParametersHelper
    {
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
