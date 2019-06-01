using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace petshelterApi.Domain
{
    public class Image
    {
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
