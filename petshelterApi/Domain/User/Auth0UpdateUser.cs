using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name = "user")]
    public class Auth0UpdateUser : Auth0BaseUser
    {
        public Auth0UpdateUser(string id, string connection)
        {
            _applicationMetaData = new ApplicationMetaData();
            Connection = connection;
            Id = id;
        }

        [DataMember(Name = "connection")]
        public string Connection { get; set; }
        [IgnoreDataMember]
        public string Id { get; set; }
    }
}
