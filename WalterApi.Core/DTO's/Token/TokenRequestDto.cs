﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalterApi.Core.DTO_s.Token
{
    public class TokenRequestDto
    {
        public string Token { get; set; }=string.Empty;
        public string RefreshToken { get; set; }= string.Empty;
    }
}
