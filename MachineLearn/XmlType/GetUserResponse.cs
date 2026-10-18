using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MachineLearn.XmlType
{
    [XmlRoot("GetUserResponse",Namespace="http://example.com/user")]
    public  class GetUserResponse
    {
        [XmlElement("name",Namespace="http://example.com/user")]
        public string Name { get; set; }
        [XmlElement("email",Namespace = "http://example.com/user")]
        public string Email {  get; set; }
    }
}
