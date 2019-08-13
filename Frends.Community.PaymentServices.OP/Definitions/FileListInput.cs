using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace Frends.Community.PaymentServices.OP.Definitions
{
    public class FileListInput
    {
        /// <summary>
        /// Url of the web service (e.g. "https://wsk.asiakastesti.op.fi/services/CorporateFileService")
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue(@"https://wsk.asiakastesti.op.fi/services/CorporateFileService")]
        public string Url { get; set; }

        /// <summary>
        /// The issuer of the Base-64 encoded X.509 certificate to be used for signing web service calls. First matching certificate is used.
        /// An Exception is thrown if a certificate is not found or it has already expired.
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string CertificateIssuedBy { get; set; }

        /// <summary>
        /// Environment to be used for the web service. Valid values are "TEST" or "PRODUCTION". Url needs to match with this option.
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"TEST\"")]
        public string Environment { get; set; }

        /// <summary>
        /// Customer Id number. The certificate needs to be assigned to the same customer id (e.g. "1234567890")
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"\"")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Request Id is the web service call id number which needs to be unique for at least 3 months
        /// </summary>
        [Required]
        [DefaultValue(0)]
        public int RequestId { get; set; }

        /// <summary>
        /// Optional parameter FileType can be used to filter filelist. Files with specific FileType will be returned (e.g. "pain.001.001.02").
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"\"")]
        public string FileType { get; set; }

        /// <summary>
        /// Optional parameter StartDate can be used to filter filelist. Files created after this will be returned (inclusive).
        /// If this value is null, or unparseable to a DateTime object, no filter is applied.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string StartDate { get; set; }

        /// <summary>
        /// Optional parameter EndDate can be used to filter filelist. Files created before this will be returned (inclusive).
        /// If this value is null no filter is applied, or unparseable to a DateTime object, no filter is applied.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string EndDate { get; set; }

        /// <summary>
        /// Optional parameter for filtering filelist. 
        /// Valid values for files sent to the bank are "WFP" (waiting for processing) or "FWD" (forwarded).
        /// Valid values for files provided by the bank are "NEW" (not yet downloaded) or "DLD" (already downloaded).
        /// Parameter "ALL" fetches all available files. If no parameter is given, "ALL" will be used.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"ALL\"")]
        public string Status { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation
        /// </summary>
        [DefaultValue(30)]
        public int ConnectionTimeOutSeconds { get; set; }
    }
}
