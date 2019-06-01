using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name = "app_metadata")]
    public class ApplicationMetaData
    {
        [DataMember(Name = "roles", EmitDefaultValue = false)]
        public string[] Roles { get; set; }
        [DataMember(Name = "shelter", EmitDefaultValue = false)]
        public string Shelter { get; set; }
    }
}
