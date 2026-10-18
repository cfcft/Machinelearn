using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MachineLearn.XmlType
{
    [XmlRoot("Envesoap", Namespace = "http://example.com/user")]
    public class Soap<T> where T:class
    {
        [XmlElement("Header", Namespace = "http://example.com/user")]
        public string Envesoap;

        [XmlElement("body")]
        public T body;
    }
}
