using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace petshelterApi.Domain.User
{
    [DataContract(Name = "user")]
    public abstract class Auth0BaseUser
    {
        [DataMember(Name = "app_metadata", EmitDefaultValue = false)]
        protected ApplicationMetaData _applicationMetaData { get; set; }
              
        [IgnoreDataMember]
        public int? ShelterId
        {
            get
            {
                if (int.TryParse(_applicationMetaData?.Shelter, out var result)) return result;
                return null;
            }
            set
            {
                if (value.HasValue) _applicationMetaData.Shelter = value.Value.ToString();
            }
        }

        [IgnoreDataMember]
        public List<string> Roles
        {
            get
            {
                if (_applicationMetaData?.Roles != null && 
                    _applicationMetaData.Roles.Length > 0)
                {
                    return new List<string>(_applicationMetaData.Roles);
                }
                return new List<string>();
            }
            set
            {
                if (value.Any()) _applicationMetaData.Roles = value.ToArray();
            }
        }
    }
}
