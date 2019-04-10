﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.PaymentServices.OP.Definitions
{
    public class UploadFileInput
    {
        /// <summary>
        /// Url of the web service (e.g. "https://wsk.asiakastesti.op.fi/services/CorporateFileService")
        /// </summary>
        [Required]
        [DefaultValue("\"\"")]
        public string Url { get; set; }

        /// <summary>
        /// The issuer of the X509Certificate2 certificate object to used for signing web service calls. First matching certificate is used.
        /// An Exception is thrown if a certificate is not found or it has already expired.
        /// </summary>
        [Required]
        [DefaultValue("")]
        public string CertificateIssuedBy { get; set; }

        /// <summary>
        /// Environment to be used for the web service. Valid values are "TEST" or "PRODUCTION". Url needs to match with this option.
        /// </summary>
        [Required]
        [DefaultValue("\"TEST\"")]
        public string Environment { get; set; }

        /// <summary>
        /// Customer Id number. The certificate needs to be assigned to the same customer id (e.g. "1234567890")
        /// </summary>
        [Required]
        [DefaultValue("\"\"")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Request Id is the web service call id number which needs to be unique for at least 3 months
        /// </summary>
        [Required]
        [DefaultValue(0)]
        public int RequestId { get; set; }

        /// <summary>
        /// File input
        /// </summary>
        [Required]
        [DefaultValue("\"\"")]
        public string FileInput { get; set; }

        /// <summary>
        /// File type to upload (e.g. "pain.001.001.02")
        /// </summary>
        [Required]
        [DefaultValue("\"\"")]
        public string FileType { get; set; }

        /// <summary>
        /// Destination file encoding, if empty UTF8 will be used
        /// </summary>
        [DefaultValue("\"utf-8\"")]
        public string FileEncoding { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation
        /// </summary>
        [DefaultValue(30)]
        public int ConnectionTimeOutSeconds { get; set; }
    }
}
