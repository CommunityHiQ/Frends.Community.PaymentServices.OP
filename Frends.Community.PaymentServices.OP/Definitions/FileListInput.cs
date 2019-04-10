﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.PaymentServices.OP.Definitions
{
    public class FileListInput
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
        /// Optional parameter FileType can be used to filter filelist. Files with specific FileType will be returned (e.g. "pain.001.001.02").
        /// </summary>
        [DefaultValue("\"\"")]
        public string FileType { get; set; }

        /// <summary>
        /// Optional parameter StartDate can be used to filter filelist. Files created after this will be returned (inclusive). If this value is null no filter is applied.
        /// </summary>
        [DefaultValue("")]
        public object StartDate { get; set; }

        /// <summary>
        /// Optional parameter EndDate can be used to filter filelist. Files created before this will be returned (inclusive). If this value is null no filter is applied.
        /// </summary>
        [DefaultValue("")]
        public object EndDate { get; set; }

        /// <summary>
        /// Optional parameter Status can be used to filter filelist.
        /// Valid values for files sent to hte bank by the customer are "WFP" or "FWD" (WFP = waiting for processing. FWD = forwarded).
        /// Valid values for files sent to hte bank by the customer are "NEW" or "DLD" (NEW = files not downloaded yet. DLD = files already downloaded).
        /// If no parameter is given or if the status is "ALL", all files will be listed.
        /// </summary>
        [DefaultValue("\"ALL\"")]
        public string Status { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation
        /// </summary>
        [DefaultValue(30)]
        public int ConnectionTimeOutSeconds { get; set; }
    }
}