using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Components
{
    public class UserData
    {
        private readonly bool _isNull = false;

        private long _userId;
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _email;
        private long _right;
        private int _status;

        private string _signature;
        private long _ticks;

        public UserData()
        {
            _isNull = true;
        }

        public UserData(long userId, string firstName, string lastName, string username, string email, long right, int status, string signature, long ticks)
        {
            _userId = userId;
            _firstName = firstName;
            _lastName = lastName;
            _username = username;
            _email = email;
            _right = right;
            _status = status;

            _signature = signature;
            _ticks = ticks;
        }
        
        public bool IsNull() { return _isNull; }
        
        public long UserId
        {
            get { return _userId; }
        }
        
        public string FirstName
        {
            get { return _firstName; }
        }
        
        public string LastName
        {
            get { return _lastName; }
        }
        
        public string Username
        {
            get { return _username; }
        }
        
        public string Email
        {
            get { return _email; }
        }
        
        public long Right
        {
            get { return _right; }
        }
        
        public int Status
        {
            get { return _status; }
        }

        public string Signature
        {
            get { return _signature; }
        }

        public long Ticks
        {
            get { return _ticks; }
        }
    }
}
