using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Ilsrep.PollApplication.Helpers
{
    class CommandLineParametersHelper
    {
        public static NameValueCollection Parse(string[] args)
        {
            String key = String.Empty;
            NameValueCollection commandlineParameters = new NameValueCollection();

            foreach (String commandlineParameter in args)
            {
                if (key == String.Empty && commandlineParameter.Substring(0, 1) == "-")
                {
                    key = commandlineParameter.Substring(1);
                }
                else if (key != String.Empty)
                {
                    commandlineParameters.Add(key, commandlineParameter);
                    key = String.Empty;
                }
            }

            return commandlineParameters;
        }
    }
}
