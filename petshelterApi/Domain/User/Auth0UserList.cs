using System.Collections.Generic;
using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name = "users")]
    public class Auth0UserList
    {
        [DataMember(Name = "users")]
        public List<Auth0ReadUser> Users { get; set; }

        [DataMember(Name = "start")]
        public int Start { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "length")]
        public int Length { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
