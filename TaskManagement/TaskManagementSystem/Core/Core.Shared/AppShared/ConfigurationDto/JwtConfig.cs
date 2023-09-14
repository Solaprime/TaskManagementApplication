using System;
using System.Collections.Generic;
using System.Text;

namespace AppShared.ConfigurationDto
{
   public  class JwtConfig
    {
        public string Secret { get; set; }
        public int TokenExpiry { get; set; }
        public string CharactersConfig { get; set; }
        //Double for Int
    }
}