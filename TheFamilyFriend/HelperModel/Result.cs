using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.HelperModel
{
    public class Result
    {
        public Result()
        {
        }
        public Result(bool _issucess, string _message) {

            this.issucess = _issucess;
            this.message = _message;
        }
        public bool issucess { get; set; }

        public string message { get; set; }

      
    }
}