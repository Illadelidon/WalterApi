﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalterApi.Core.Entities.User;
using WalterApi.Core.Interfaces;

namespace WalterApi.Core.Entities
{
    public class RefreshToken : IEntity
    {
        public int Id { get; set ; }
        public string UserId { get; set ; }=string.Empty;
        public string Token { get; set ; }=string.Empty;
        public string JwtId { get; set ; }=string.Empty;
        public bool IsUsed { get; set ; }
        public bool IsRevoked { get; set ; }
        public DateTime AddedDate { get; set ; }
        public DateTime ExpireDate { get; set ; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set ; }

    }
}
