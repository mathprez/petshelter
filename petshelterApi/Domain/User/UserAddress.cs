using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name ="adress")]
    public class UserAddress
    {
        [DataMember(Name ="line_one")]
        public string LineOne { get; set; } = string.Empty;
        [DataMember(Name = "line_two")]
        public string LineTwo { get; set; } = string.Empty;
        [DataMember(Name = "line_three")]
        public string LineThree { get; set; } = string.Empty;
        [DataMember(Name = "country")]
        public string Country { get; set; } = string.Empty;
    }
}
