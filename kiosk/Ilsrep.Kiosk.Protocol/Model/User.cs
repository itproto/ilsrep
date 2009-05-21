using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.Kiosk.Protocol.Model
{
    public class User
    {
        /// <summary>
        /// User ID in DB
        /// </summary>
        private int _id;
        /// <summary>
        /// User password(access code)
        /// </summary>
        private string _password;
        /// <summary>
        /// Number of credits available
        /// </summary>
        private int _credits;
        /// <summary>
        /// Shows is user an admin or no
        /// </summary>
        private bool _isAdmin;

        /// <summary>
        /// User constructor that initialize all values to 0, false or empty string
        /// </summary>
        public User()
        {
            _password = String.Empty;
            _credits = 0;
            _isAdmin = false;
        }

        /// <summary>
        /// User constructor that initialize all values
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="credits">User credits</param>
        /// <param name="isAdmin">Is user an admin</param>
        public User(string password, int credits, bool isAdmin)
        {
            Password = password;
            Credits = credits;
            IsAdmin = isAdmin;
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

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != String.Empty)
                {
                    _password = value;
                }
                else
                {
                    throw new Exception("Password can't be empty");
                }
            }
        }

        public int Credits
        {
            get
            {
                return _credits;
            }
            set
            {
                _credits = value;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _isAdmin;
            }
            set
            {
                _isAdmin = value;
            }
        }
    }
}
