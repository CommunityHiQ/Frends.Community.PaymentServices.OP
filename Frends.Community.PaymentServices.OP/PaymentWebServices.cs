using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Newtonsoft.Json.Linq;
using Frends.Community.PaymentServices.OP.Definitions;
using Frends.Community.PaymentServices.OP.Helpers;
using Frends.Community.PaymentServices.OP.Services;
using Environment = Frends.Community.PaymentServices.OP.Helpers.Enums.Environment;
using Status = Frends.Community.PaymentServices.OP.Helpers.Enums.Status;

#pragma warning disable 1591

namespace Frends.Community.PaymentServices.OP
{
    public class WebServices
    {
        /// <summary>
        /// DownloadFileList fetches a filelist from the web service. 
        /// Files can be filtered with different parameters: FileType, StartDate, EndDate or Status.
        /// In case of an error an exception is thrown.
        /// </summary>
        /// <returns>JToken array. Properties: FileReference, TargetId, ParentFileReference, FileType, FileTimestamp, Status</returns>
        public static JToken DownloadFileList(FileListInput input, CancellationToken cancellationToken)
        {
            string customerId = input.CustomerId;
            string environment = input.Environment;
            string status = input.Status;
            string url = input.Url;

            var stringParameters = new[]
            {
                new KeyValuePair<string, string>("customerId", customerId)
            };

            // Fetches certificate based on issuer
            X509Certificate2 cert = CertificateService.FindCertificate(input.CertificateIssuedBy);

            // Validate parameters
            Validators.ValidateParameters(url, cert, environment, stringParameters);

            if (!string.IsNullOrEmpty(status))
            {
                Validators.ValidateStatusParameter(status);
            }

            var env = (Environment)Enum.Parse(typeof(Environment), environment);
            var fileStatus = string.IsNullOrEmpty(status) ? Status.ALL : (Status)Enum.Parse(typeof(Status), status);
            var startDateParam = input.StartDate.ResolveDate();
            var endDateParam = input.EndDate.ResolveDate();

            var message = MessageService.GetDownloadFileListMessage(cert, customerId, input.FileType, startDateParam, endDateParam, fileStatus, env, input.RequestId);
            var result = WebService.CallWebService(url, message, MessageService.SoftwareId, input.ConnectionTimeOutSeconds, cancellationToken);
            string resultXml = result.Result.Body;
            var applicationResponse = CheckResultForErrorsAndReturnApplicationResult(resultXml);

            return Helper.GetFileListResultFromResponseXml(applicationResponse);
        }

        /// <summary>
        /// Uploads a file to the web service. 
        /// File type needs to be specified for the file.
        /// File content is compressed by GZIP.
        /// In case of an error an exception is thrown.
        /// </summary>
        /// <returns>JToken. Properties: FileReference, TargetId, ParentFileReference, FileType, FileTimestamp, Status</returns>
        public static JToken UploadFile(UploadFileInput input, CancellationToken cancellationToken)
        {
            string customerId = input.CustomerId;
            string environment = input.Environment;
            string fileInput = input.FileInput;
            string fileType = input.FileType;
            string url = input.Url;

            var stringParameters = new[]
            {
                new KeyValuePair<string, string>("customerId", customerId),
                new KeyValuePair<string, string>("fileInput", fileInput),
                new KeyValuePair<string, string>("fileType", fileType)
            };

            X509Certificate2 cert = CertificateService.FindCertificate(input.CertificateIssuedBy);

            Validators.ValidateParameters(input.Url, cert, environment, stringParameters);

            var env = (Environment)Enum.Parse(typeof(Environment), environment);
            
            var encoding = string.IsNullOrEmpty(input.FileEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(input.FileEncoding);

            var message = MessageService.GetUploadFileMessage(cert, customerId, env, input.RequestId, fileInput, fileType, encoding);
            var result = WebService.CallWebService(url, message, MessageService.SoftwareId, input.ConnectionTimeOutSeconds, cancellationToken);
            string resultXml = result.Result.Body;
            var applicationResponse = CheckResultForErrorsAndReturnApplicationResult(resultXml);

            return Helper.GetFileInfoFromResponseXml(applicationResponse);
        }

        /// <summary>
        /// Deletes a file from web service.
        /// In case of an error an exception is thrown.
        /// </summary>
        /// <returns>Returns an empty string</returns>
        public static DeleteFileOutput DeleteFile(DeleteFileInput input, CancellationToken cancellationToken)
        {
            string customerId = input.CustomerId;
            string environment = input.Environment;
            string fileReference = input.FileReference;
            string url = input.Url;

            var stringParameters = new[]
            {
                new KeyValuePair<string, string>("customerId", customerId),
                new KeyValuePair<string, string>("fileReference", fileReference)
            };

            X509Certificate2 cert = CertificateService.FindCertificate(input.CertificateIssuedBy);

            Validators.ValidateParameters(url, cert, environment, stringParameters);

            var env = (Environment)Enum.Parse(typeof(Environment), environment);
            //var cert = (X509Certificate2)certificate;
            var message = MessageService.GetDeleteFileMessage(cert, customerId, env, input.RequestId, fileReference);
            var result = WebService.CallWebService(url, message, MessageService.SoftwareId, input.ConnectionTimeOutSeconds, cancellationToken);
            var resultXml = result.Result.Body;
            return new DeleteFileOutput { ApplicationResult = CheckResultForErrorsAndReturnApplicationResult(resultXml) };
        }

        /// <summary>
        /// Downloads a file from web service.
        /// In case of an error an exception is thrown.
        /// </summary>
        /// <returns>Returns the downloaded file content</returns>
        public static DownLoadFileOutput DownloadFile(DownloadFileInput input, CancellationToken cancellationToken)
        {
            string customerId = input.CustomerId;
            string environment = input.Environment;
            string fileEncoding = input.FileEncoding;
            string fileReference = input.FileReference;
            string url = input.Url;
            
            var stringParameters = new[]
            {
                new KeyValuePair<string, string>("customerId", customerId),
                new KeyValuePair<string, string>("fileReference", fileReference)
            };

            X509Certificate2 cert = CertificateService.FindCertificate(input.CertificateIssuedBy);

            Validators.ValidateParameters(url, cert, environment, stringParameters);

            var env = (Environment)Enum.Parse(typeof(Environment), environment);
            //var cert = (X509Certificate2)certificate;
            var encoding = string.IsNullOrEmpty(fileEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(fileEncoding);

            var message = MessageService.GetDownloadFileMessage(cert, customerId, env, input.RequestId, fileReference);
            var result = WebService.CallWebService(url, message, MessageService.SoftwareId, input.ConnectionTimeOutSeconds, cancellationToken);
            var resultXml = result.Result.Body;
            var applicationResponse = CheckResultForErrorsAndReturnApplicationResult(resultXml);

            return new DownLoadFileOutput { FileContent = Helper.GetFileFromResponsexml(applicationResponse, encoding) };
        }

        private static string CheckResultForErrorsAndReturnApplicationResult(string resultXml)
        {
            var applicationResponse = Helper.GetApplicationResponseXml(resultXml);

            if (string.IsNullOrEmpty(applicationResponse))
            {
                throw new Exception($"Server returned: {resultXml}");
            }
            if (!Helper.CheckIfCallWasSuccessful(resultXml))
            {
                throw new Exception(Helper.GetErrorMessage(applicationResponse));
            }
            // soap envelope signature is not checked as .Net does not understand BinarySecurityToken certificate
            if (!Helper.VerifyApplicationResponseSignature(applicationResponse))
            {
                throw new Exception($"Application response signature does not match to content! Response: {applicationResponse}");
            }

            return applicationResponse;
        }
    }
}
