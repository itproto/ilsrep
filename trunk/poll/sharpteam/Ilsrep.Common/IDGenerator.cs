using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.Common
{
    /// <summary>
    /// IDGenerator
    /// </summary>
    public class IDGenerator
    {
        private int _id;

        public IDGenerator()
        {
            _id = 0;
        }

        public int id
        {
            get
            {
                return --_id;
            }
        }
    }
}