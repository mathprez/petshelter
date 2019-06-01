using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name = "user")]
    public class Auth0ReadUser : Auth0BaseUser
    {
        [DataMember(Name = "user_id", IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Name = "email", IsRequired = true)]
        public string Email { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "identities", IsRequired = true)]
        private Identity[] _identities { get; set; }

        [IgnoreDataMember]
        public string Connection
        {
            get => _identities[0].Connection;
        }

        [DataContract(Name = "identity")]
        internal class Identity
        {
            [DataMember(Name = "connection")]
            public string Connection { get; set; }
        }
    }
}
