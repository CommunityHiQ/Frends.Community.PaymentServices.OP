namespace Frends.Community.PaymentServices.OP.Definitions
{
    public class FileInfo
    {
        public string FileReference { get; set; }

        public string FileType { get; set; }

        public string TargetId { get; set; }

        public string FileTimestamp { get; set; }

        public string Status { get; set; }

        public string ForwardedTimestamp { get; set; }

        public override string ToString()
        {
            return $"{FileReference} - {FileType} - {FileTimestamp} - {Status}";
        }
    }
}
