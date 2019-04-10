using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Environment = Frends.Community.PaymentServices.OP.Helpers.Enums.Environment;
using Status = Frends.Community.PaymentServices.OP.Helpers.Enums.Status;

#pragma warning disable 1591

namespace Frends.Community.PaymentServices.OP.Services
{
    // This class contains methods that - with the use of methods in the Helper.cs - construct SOAP 
    // request XML messages to be sent to the web service interface
    public static class MessageService
    {
        public static string SoftwareId => "FRENDS";

        private static readonly XNamespace ModelNs = "http://model.bxd.fi";
        private static readonly XNamespace CorporateNs = "http://bxd.fi/CorporateFileService";
        private static readonly XNamespace DataNs = "http://bxd.fi/xmldata/";
        private static readonly XNamespace EnvNs = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace WsuNs = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        private static readonly XNamespace WsseNs = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        private static readonly Dictionary<string, XNamespace> Namespaces = new Dictionary<string, XNamespace>
                                                                {
                                                                    { "mod", ModelNs },
                                                                    { "cor", CorporateNs },
                                                                    { "dat", DataNs },
                                                                    { "env", EnvNs },
                                                                    { "wsu", WsuNs },
                                                                    { "wsse", WsseNs },
                                                                };

        private static readonly XNamespace OpcNs = "http://mlp.op.fi/OPCertificateService";
        private static readonly XNamespace OpDataNs = "http://op.fi/mlp/xmldata/";

        private static readonly Dictionary<string, XNamespace> FewNamespaces = new Dictionary<string, XNamespace>
                                                                {
                                                                    { "env", EnvNs },
                                                                    { "opc", OpcNs },
                                                                    { "dat", OpDataNs }
                                                                };

        public static string GetDownloadFileListMessage(X509Certificate2 certificate, string customerId, string fileType, DateTime? startDate, DateTime? endDate, Status status, Environment environment, int requestId)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetDownloadFileListRequest(
                customerId: customerId,
                timestamp: timestamp,
                fileType: fileType,
                startDate: startDate,
                endDate: endDate,
                status: status,
                environment: environment,
                softwareId: SoftwareId,
                requestId: requestId,
                certificate: certificate,
                namespaces: Namespaces);

            var soapRequestDocument = Helper.GetSoapRequestXmlDocument(certificate, soapBodyContent, timestamp, Namespaces);

            return soapRequestDocument.OuterXml;
        }

        public static string GetDeleteFileMessage(X509Certificate2 certificate, string customerId, Environment environment, int requestId, string fileReference)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetDeleteFileRequest(
                customerId: customerId,
                timestamp: timestamp,
                environment: environment,
                softwareId: SoftwareId,
                requestId: requestId,
                fileReference: fileReference,
                certificate: certificate,
                namespaces: Namespaces);

            var soapRequestDocument = Helper.GetSoapRequestXmlDocument(certificate, soapBodyContent, timestamp, Namespaces);

            return soapRequestDocument.OuterXml;
        }

        public static string GetDownloadFileMessage(X509Certificate2 certificate, string customerId, Environment environment, int requestId, string fileReference)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetDownloadFileRequest(
                customerId: customerId,
                timestamp: timestamp,
                environment: environment,
                softwareId: SoftwareId,
                requestId: requestId,
                fileReference: fileReference,
                certificate: certificate,
                namespaces: Namespaces);

            var soapRequestDocument = Helper.GetSoapRequestXmlDocument(certificate, soapBodyContent, timestamp, Namespaces);

            return soapRequestDocument.OuterXml;
        }

        // This message could be used to query user information from web service, but OKO does not support this just yet.
        public static string GetUserInfoMessage(X509Certificate2 certificate, string customerId, Environment environment, int requestId)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetUserInfoRequest(
                customerId: customerId,
                timestamp: timestamp,
                environment: environment,
                softwareId: SoftwareId,
                requestId: requestId,
                certificate: certificate,
                namespaces: Namespaces);

            var soapRequestDocument = Helper.GetSoapRequestXmlDocument(certificate, soapBodyContent, timestamp, Namespaces);

            return soapRequestDocument.OuterXml;
        }

        public static string GetUploadFileMessage(X509Certificate2 certificate, string customerId, Environment environment, int requestId, string fileInput, string fileType, Encoding fileEncoding)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetUploadFileRequest(
                customerId: customerId,
                timestamp: timestamp,
                environment: environment,
                softwareId: SoftwareId,
                requestId: requestId,
                certificate: certificate,
                namespaces: Namespaces,
                fileInput: fileInput,
                fileType: fileType,
                fileEncoding: fileEncoding);

            var soapRequestDocument = Helper.GetSoapRequestXmlDocument(certificate, soapBodyContent, timestamp, Namespaces);

            return soapRequestDocument.OuterXml;
        }

        // This has not yet been implemented
        public static string GetCertificateMessage(string customerId, Environment environment, int requestId, string transferKey, string pkcs10)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetCertificateRequest(
                    customerId: customerId,
                    timestamp: timestamp,
                    environment: environment,
                    softwareId: SoftwareId,
                    requestId: requestId,
                    namespaces: FewNamespaces,
                    transferKey: transferKey,
                    pkcs10: pkcs10);

            var soapRequestDocument = Helper.GetEmptySoapEnvelope(soapBodyContent, timestamp, FewNamespaces);

            return soapRequestDocument.OuterXml;
        }

        // This has not yet been implemented
        public static string GetCertificatesMessage(string customerId, Environment environment, int requestId, string transferKey)
        {
            var timestamp = DateTime.Now;

            var soapBodyContent = Helper.GetCertificatesRequest(
                    customerId: customerId,
                    timestamp: timestamp,
                    environment: environment,
                    softwareId: SoftwareId,
                    requestId: requestId,
                    namespaces: FewNamespaces,
                    transferKey: transferKey);

            var soapRequestDocument = Helper.GetEmptySoapEnvelope(soapBodyContent, timestamp, FewNamespaces);

            return soapRequestDocument.OuterXml;
        }
    }
}
