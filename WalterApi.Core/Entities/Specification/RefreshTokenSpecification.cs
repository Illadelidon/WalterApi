using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalterApi.Core.Entities.Specification
{
    public static class RefreshTokenSpecification
    {
        public  class GetRefreshToken : Specification<RefreshToken>
        {
            public GetRefreshToken(string refreshtoken)
            {
                Query.Where(t => t.Token == refreshtoken);
            }
        }
    }
}
