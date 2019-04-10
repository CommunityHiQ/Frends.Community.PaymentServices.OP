using System.Linq;
using System.Xml;
using Frends.Community.PaymentServices.OP.Definitions;

namespace Frends.Community.PaymentServices.OP.Helpers
{
    public static class XmlNodeExtensions
    {
        public static FileInfo GetFileInfo(this XmlNode node)
        {
            var children = node.ChildNodes.Cast<XmlNode>().ToList();

            return new FileInfo
            {
                FileReference = children.FirstOrDefault(e => e.Name == "FileReference")?.InnerText,
                FileType = children.FirstOrDefault(e => e.Name == "FileType")?.InnerText,
                TargetId = children.FirstOrDefault(e => e.Name == "TargetId")?.InnerText,
                FileTimestamp = children.FirstOrDefault(e => e.Name == "FileTimestamp")?.InnerText,
                Status = children.FirstOrDefault(e => e.Name == "Status")?.InnerText,
                ForwardedTimestamp = children.FirstOrDefault(e => e.Name == "ForwardedTimestamp")?.InnerText
            };
        }
    }
}
