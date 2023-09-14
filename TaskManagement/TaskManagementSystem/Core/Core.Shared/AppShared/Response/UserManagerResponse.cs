using System;
using System.Collections.Generic;
using System.Text;

namespace AppShared.Response
{
   public class UserManagerResponse : BaseResponse
    {
        public UserManagerResponse()
        {
            ObjectToReturn = new object();
        }
        public object ObjectToReturn { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Id { get; set; }
        public IEnumerable<string> Error { get; set; }
        public DateTime? ExpiredDate { get; set; }
   }
}
