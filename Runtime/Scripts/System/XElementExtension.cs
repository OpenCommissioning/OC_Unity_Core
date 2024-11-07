using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OC
{
    public static class XElementExtension
    {
        public static XElement ToXElement<T>(this object obj)
        {
            using var memoryStream = new MemoryStream();
            using TextWriter streamWriter = new StreamWriter(memoryStream);
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(streamWriter, obj);
            var element = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
            element.RemoveAttributes();
            return element;
        }

        public static T FromXElement<T>(this XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }
    }
}