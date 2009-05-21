using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.Kiosk.Protocol.Model
{
    public class Program
    {
        /// <summary>
        /// Program ID in DB
        /// </summary>
        private int _id;
        /// <summary>
        /// Program name that will be shown in user page
        /// </summary>
        private string _name;
        /// <summary>
        /// Program physical path
        /// </summary>
        private string _path;

        /// <summary>
        /// Program constructor that initialize all values to empty string
        /// </summary>
        public Program()
        {
            _name = String.Empty;
            _path = String.Empty;
        }

        /// <summary>
        /// Program constructor that initialize all values
        /// </summary>
        /// <param name="name">Program name</param>
        /// <param name="path">Program physical path</param>
        public Program(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != String.Empty)
                {
                    _name = value;
                }
                else
                {
                    throw new Exception("Program name can't be empty");
                }
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value != String.Empty)
                {
                    _path = value;
                }
                else
                {
                    throw new Exception("Program path can't be empty");
                }
            }
        }
    }
}
