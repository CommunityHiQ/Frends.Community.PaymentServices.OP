﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Frends.Community.PaymentServices.OP.Helpers;
using FileInfo = Frends.Community.PaymentServices.OP.Definitions.FileInfo;
using Environment = Frends.Community.PaymentServices.OP.Helpers.Enums.Environment;
using Status = Frends.Community.PaymentServices.OP.Helpers.Enums.Status;

#pragma warning disable 1591

namespace Frends.Community.PaymentServices.OP.Services
{
    // These are helper methods called from elsewhere, e.g. MessageService.cs
    public static class Helper
    {
        // Builds the SOAP request XML by adding signature node to the SOAP request with SOAP envelope and body 
        public static XmlDocument GetSoapRequestXmlDocument(X509Certificate2 certificate, XElement soapBodyContent, DateTime timestamp, Dictionary<string, XNamespace> namespaces)
        {
            var bodyReferenceId = $"bodyId-{Guid.NewGuid()}";
            var timestampReferenceId = $"timestampId-{Guid.NewGuid()}";
            var tokenReferenceId = $"tokenId-{Guid.NewGuid()}";

            var result = GetSoapRequest(certificate, timestamp, soapBodyContent, bodyReferenceId, timestampReferenceId, tokenReferenceId, namespaces);
            var soapRequestDocument = GetXmlDocument(result.ToString());

            var references = new[] { bodyReferenceId, timestampReferenceId };
            var keyInfo = GetKeyInfoWithSecurityToken(tokenReferenceId, namespaces);

            var signatureNode = GetSignatureNode(soapRequestDocument, certificate, references, keyInfo);
            var securityNode = soapRequestDocument.SelectSingleNode("/env:Envelope/env:Header/wsse:Security", GetNamespaceManagerFromNamespaces(soapRequestDocument, namespaces));
            var nodeToImport = securityNode.OwnerDocument.ImportNode(signatureNode, true);
            securityNode.AppendChild(nodeToImport);

            return soapRequestDocument;
        }

        // Builds SOAP request (without signature node) from header and body
        private static XElement GetSoapRequest(X509Certificate2 certificate, DateTime timestamp, XElement bodyContent, string bodyReferenceId, string timestampReferenceId, string tokenReferenceId, Dictionary<string, XNamespace> namespaces)
        {
            var soapHeader = GetSoapHeader(certificate, timestamp, timestampReferenceId, tokenReferenceId, namespaces);
            var soapBody = new XElement(namespaces["env"] + "Body", new XAttribute(namespaces["wsu"] + "Id", bodyReferenceId),
                bodyContent);

            return new XElement(namespaces["env"] + "Envelope", ConvertNamespacesToAttributes(namespaces),
                                    soapHeader,
                                    soapBody);
        }

        // Creates application request for fetching user info
        private static XElement GetUserInfoApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, Dictionary<string, XNamespace> namespaces)
        {
            return new XElement(namespaces["dat"] + "ApplicationRequest",
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "SoftwareId", softwareId));
        }

        // Creates application request for uploading a file
        private static XElement GetUploadFileApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, Dictionary<string, XNamespace> namespaces, string fileInput, string fileType, Encoding fileEncoding)
        {
            var compressedFile = GzipAndBase64Encode(fileInput, fileEncoding);

            return new XElement(namespaces["dat"] + "ApplicationRequest",
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "TargetId", "TARGET"),     // this needs to hardcoded to this value
                new XElement(namespaces["dat"] + "Compression", true),      // this needs to be true since the content is compressed with Gzip
                new XElement(namespaces["dat"] + "SoftwareId", softwareId),
                new XElement(namespaces["dat"] + "FileType", fileType),
                new XElement(namespaces["dat"] + "Content", compressedFile));
        }

        // Creates application request for deleting a file
        private static XElement GetDeleteFileApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, string fileReference, Dictionary<string, XNamespace> namespaces)
        {
            return new XElement(namespaces["dat"] + "ApplicationRequest",
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "FileReferences",
                    new XElement(namespaces["dat"] + "FileReference", fileReference)),
                new XElement(namespaces["dat"] + "SoftwareId", softwareId));
        }

        // Creates application request for getting a file
        private static XElement GetFileApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, string fileReference, Dictionary<string, XNamespace> namespaces)
        {
            return new XElement(namespaces["dat"] + "ApplicationRequest",
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "FileReferences",
                    new XElement(namespaces["dat"] + "FileReference", fileReference)),
                new XElement(namespaces["dat"] + "Compression", true),
                new XElement(namespaces["dat"] + "SoftwareId", softwareId));
        }

        // Creates application request for getting a list of available files
        private static XElement GetFileListApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, string fileType, DateTime? startDate, DateTime? endDate, Status status, Dictionary<string, XNamespace> namespaces)
        {
            var properties = new List<XElement>
            {
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                startDate.HasValue ? new XElement(namespaces["dat"] + "StartDate", startDate.Value.ToString("yyyy-MM-dd")) : null,
                endDate.HasValue ? new XElement(namespaces["dat"] + "EndDate", endDate.Value.ToString("yyyy-MM-dd")) : null,
                new XElement(namespaces["dat"] + "Status", status.ToString()),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "SoftwareId", softwareId),
                !string.IsNullOrEmpty(fileType) ? new XElement(namespaces["dat"] + "FileType", fileType) : null
        };

            return new XElement(namespaces["dat"] + "ApplicationRequest", properties.Where(p => p != null));
        }

        // Builds the SOAP envelope's RequestHeader element
        private static XElement GetRequestHeader(string senderId, int requestId, DateTime timestamp, string softwareId, Dictionary<string, XNamespace> namespaces)
        {
            return new XElement(namespaces["mod"] + "RequestHeader",
                new XElement(namespaces["mod"] + "SenderId", senderId),
                new XElement(namespaces["mod"] + "RequestId", requestId),
                new XElement(namespaces["mod"] + "Timestamp", timestamp),
                new XElement(namespaces["mod"] + "Language", "FI"),
                new XElement(namespaces["mod"] + "UserAgent", softwareId),
                new XElement(namespaces["mod"] + "ReceiverId", "OKOYFIHH"));
        }

        // Builds SOAP message header
        private static XElement GetSoapHeader(X509Certificate2 certificate, DateTime timestamp, string timestampReferenceId, string tokenReferenceId, Dictionary<string, XNamespace> namespaces)
        {
            var publicCert = new KeyInfoX509Data(certificate).GetXml().InnerText;

            return new XElement(namespaces["env"] + "Header",
                new XElement(namespaces["wsse"] + "Security", new XAttribute(namespaces["env"] + "mustUnderstand", 1),
                    new XElement(namespaces["wsse"] + "BinarySecurityToken",
                        new XAttribute("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"),
                        new XAttribute("ValueType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"),
                        new XAttribute(namespaces["wsu"] + "Id", tokenReferenceId),
                        publicCert),
                    new XElement(namespaces["wsu"] + "Timestamp", new XAttribute(namespaces["wsu"] + "Id", timestampReferenceId),
                        new XElement(namespaces["wsu"] + "Created", timestamp.ToUniversalTime().ToString("s") + "Z"),
                        new XElement(namespaces["wsu"] + "Expires", timestamp.AddMinutes(5).ToUniversalTime().ToString("s") + "Z"))));
        }

        private static XmlDocument GetXmlDocument(string input)
        {
            var result = new XmlDocument { PreserveWhitespace = true };
            result.LoadXml(input);

            return result;
        }

        private static string GetBase64String(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        private static string GetUtf8StringFromBase64(string base64)
        {
            return string.IsNullOrEmpty(base64) ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        private static string SignApplicationRequestAndReturnAsBase64(XElement applicationRequest, X509Certificate2 certificate)
        {
            var requestAsDocument = GetXmlDocument(applicationRequest.ToString());
            var keyInfo = GetKeyInfoWithCert(certificate);
            var signedObject = GetSignatureNode(requestAsDocument, certificate, new List<string>(), keyInfo);

            var node = requestAsDocument.ImportNode(signedObject, true);
            requestAsDocument.DocumentElement?.AppendChild(node);

            return GetBase64String(requestAsDocument.OuterXml);
        }

        // Builds SOAP request body for getting a list of available files 
        public static XElement GetDownloadFileListRequest(string customerId, DateTime timestamp, string fileType, DateTime? startDate, DateTime? endDate, Status status, Environment environment, string softwareId, int requestId, X509Certificate2 certificate, Dictionary<string, XNamespace> namespaces)
        {
            var applicationRequest = GetFileListApplicationRequest(customerId, timestamp, environment, softwareId, fileType, startDate, endDate, status, namespaces);
            var requestBase64 = SignApplicationRequestAndReturnAsBase64(applicationRequest, certificate);
            var requestHeader = GetRequestHeader(customerId, requestId, timestamp, softwareId, namespaces);

            return new XElement(namespaces["cor"] + "downloadFileListin",
                requestHeader,
                new XElement(namespaces["mod"] + "ApplicationRequest", requestBase64));
        }

        // Builds SOAP request body for deleting a file
        public static XElement GetDeleteFileRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, string fileReference, X509Certificate2 certificate, Dictionary<string, XNamespace> namespaces)
        {
            var applicationRequest = GetDeleteFileApplicationRequest(customerId, timestamp, environment, softwareId, fileReference, namespaces);
            var requestBase64 = SignApplicationRequestAndReturnAsBase64(applicationRequest, certificate);
            var requestHeader = GetRequestHeader(customerId, requestId, timestamp, softwareId, namespaces);

            return new XElement(namespaces["cor"] + "deleteFilein",
                requestHeader,
                new XElement(namespaces["mod"] + "ApplicationRequest", requestBase64));
        }

        // Builds SOAP request body for downloading a file
        public static XElement GetDownloadFileRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, string fileReference, X509Certificate2 certificate, Dictionary<string, XNamespace> namespaces)
        {
            var applicationRequest = GetFileApplicationRequest(customerId, timestamp, environment, softwareId, fileReference, namespaces);
            var requestBase64 = SignApplicationRequestAndReturnAsBase64(applicationRequest, certificate);
            var requestHeader = GetRequestHeader(customerId, requestId, timestamp, softwareId, namespaces);

            return new XElement(namespaces["cor"] + "downloadFilein",
                requestHeader,
                new XElement(namespaces["mod"] + "ApplicationRequest", requestBase64));
        }

        // Builds SOAP request body for getting user info
        public static XElement GetUserInfoRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, X509Certificate2 certificate, Dictionary<string, XNamespace> namespaces)
        {
            var applicationRequest = GetUserInfoApplicationRequest(customerId, timestamp, environment, softwareId, namespaces);
            var requestBase64 = SignApplicationRequestAndReturnAsBase64(applicationRequest, certificate);
            var requestHeader = GetRequestHeader(customerId, requestId, timestamp, softwareId, namespaces);

            return new XElement(namespaces["cor"] + "getUserInfoin",
                requestHeader,
                new XElement(namespaces["mod"] + "ApplicationRequest", requestBase64));
        }

        // Builds SOAP request body for uploading a file
        public static XElement GetUploadFileRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, X509Certificate2 certificate, Dictionary<string, XNamespace> namespaces, string fileInput, string fileType, Encoding fileEncoding)
        {
            var applicationRequest = GetUploadFileApplicationRequest(customerId, timestamp, environment, softwareId, namespaces, fileInput, fileType, fileEncoding);
            var requestBase64 = SignApplicationRequestAndReturnAsBase64(applicationRequest, certificate);
            var requestHeader = GetRequestHeader(customerId, requestId, timestamp, softwareId, namespaces);

            return new XElement(namespaces["cor"] + "uploadFilein",
                requestHeader,
                new XElement(namespaces["mod"] + "ApplicationRequest", requestBase64));
        }

        //Builds signature XML element
        private static XmlElement GetSignatureNode(XmlDocument document, X509Certificate2 certificate, IEnumerable<string> referenceIds, KeyInfo keyInfo)
        {
            var signedXmlA = new SignedXmlWithId(document) { SigningKey = certificate.PrivateKey };

            if (referenceIds.Any())
            {
                signedXmlA.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";

                foreach (var referenceId in referenceIds)
                {
                    var transform = new XmlDsigExcC14NTransform();
                    var reference = new Reference { Uri = $"#{referenceId}" };
                    reference.AddTransform(transform);
                    signedXmlA.AddReference(reference);
                }
            }
            else
            {
                signedXmlA.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

                var transform = new XmlDsigEnvelopedSignatureTransform();
                var reference = new Reference { Uri = string.Empty };     // this value needs to be empty not null
                reference.AddTransform(transform);
                signedXmlA.AddReference(reference);
            }

            signedXmlA.KeyInfo = keyInfo;
            signedXmlA.ComputeSignature();

            return signedXmlA.GetXml();
        }

        private static KeyInfo GetKeyInfoWithCert(X509Certificate2 certificate)
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));
            return keyInfo;
        }

        private static KeyInfo GetKeyInfoWithSecurityToken(string reference, Dictionary<string, XNamespace> namespaces)
        {
            var infoXml = new XElement("KeyInfo",
                new XElement(namespaces["wsse"] + "SecurityTokenReference",
                    new XElement(namespaces["wsse"] + "Reference",
                        new XAttribute("URI", $"#{reference}"),
                        new XAttribute("ValueType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"))));

            var document = GetXmlDocument(infoXml.ToString());
            var keyInfo = new KeyInfo();
            keyInfo.LoadXml(document.DocumentElement);

            return keyInfo;
        }

        private static List<XAttribute> ConvertNamespacesToAttributes(Dictionary<string, XNamespace> namespaces)
        {
            return namespaces.Select(n => new XAttribute(XNamespace.Xmlns + n.Key, n.Value.NamespaceName)).ToList();
        }

        private static XmlNamespaceManager GetNamespaceManagerFromNamespaces(XmlDocument document, Dictionary<string, XNamespace> namespaces)
        {
            var result = new XmlNamespaceManager(document.NameTable);

            foreach (var ns in namespaces)
            {
                result.AddNamespace(ns.Key, ns.Value.NamespaceName);
            }

            return result;
        }

        public static string GetContentFromBase64(string input, bool compressedWithGzip, Encoding fileEncoding)
        {
            var input64 = Convert.FromBase64String(input);

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(input64, 0, input64.Length);
                memoryStream.Position = 0L;

                if (compressedWithGzip)
                {
                    using (var resultStream = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(memoryStream, CompressionMode.Decompress))
                        {
                            gzip.CopyTo(resultStream);
                            // Here the memoryStream is disposed with gzip
                            return ReadStream(resultStream, fileEncoding);
                        }
                    }
                }
                // Here we dispose memoryStream again, but this should not create issues
                return ReadStream(memoryStream, fileEncoding);
            }
        }

        private static string GzipAndBase64Encode(string input, Encoding fileEncoding)
        {
            var inputByteArray = fileEncoding.GetBytes(input);

            using (var resultStream = new MemoryStream())
            {
                using (var gzip = new GZipStream(resultStream, CompressionMode.Compress))
                {
                    // Disposing of gzip also disposes resultStream
                    gzip.Write(inputByteArray, 0, inputByteArray.Length);
                }

                var resultBytes = resultStream.ToArray();
                // Here we dispose resultStream again, but this should not create issues
                return Convert.ToBase64String(resultBytes);
            }
        }

        private static string ReadStream(Stream stream, Encoding fileEncoding)
        {
            stream.Position = 0L;

            using (var reader = new StreamReader(stream, fileEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        private static XElement FindElement(string xml, string elementName)
        {
            var element = XElement.Parse(xml);
            return element.DescendantNodesAndSelf().Where(n => n is XElement).Cast<XElement>().FirstOrDefault(e => e.Name.LocalName == elementName);
        }

        public static string GetApplicationResponseXml(string responseXml)
        {
            var response = FindElement(responseXml, "ApplicationResponse");

            return GetUtf8StringFromBase64(response?.Value);
        }

        public static bool VerifyApplicationResponseSignature(string applicationResponseXml)
        {
            var document = new XmlDocument();
            document.LoadXml(applicationResponseXml);

            var signedXml = new SignedXmlWithId(document);
            var signatureElement = document.GetElementsByTagName("Signature").Cast<XmlElement>().First();
            signedXml.LoadXml(signatureElement);

            return signedXml.CheckSignature();
        }

        public static bool CheckIfCallWasSuccessful(string responseXml)
        {
            var response = FindElement(responseXml, "ResponseCode");

            return response?.Value == "00";
        }

        public static string GetFileFromResponsexml(string applicationResponseXml, Encoding fileEncoding)
        {
            var content = FindElement(applicationResponseXml, "Content")?.Value;
            var compressed = bool.Parse(FindElement(applicationResponseXml, "Compressed")?.Value ?? "false");

            return GetContentFromBase64(content, compressed, fileEncoding);
        }

        public static List<FileInfo> GetFileListResultFromResponseXml(string applicationResponseXml)
        {
            var document = new XmlDocument();
            document.LoadXml(applicationResponseXml);

            var files = new List<FileInfo>();

            foreach (var fileDescription in document.GetElementsByTagName("FileDescriptor").Cast<XmlNode>().ToList())
            {
                files.Add(fileDescription.GetFileInfo());
            }

            return files;
        }

        public static FileInfo GetFileInfoFromResponseXml(string applicationResponseXml)
        {
            var document = new XmlDocument();
            document.LoadXml(applicationResponseXml);

            var fileDescription = document.GetElementsByTagName("FileDescriptor").Cast<XmlNode>().FirstOrDefault();

            return fileDescription?.GetFileInfo();
        }

        public static string GetErrorMessage(string applicationResponseXml)
        {
            var responseCode = FindElement(applicationResponseXml, "ResponseCode")?.Value;
            var responseText = FindElement(applicationResponseXml, "ResponseText")?.Value;
            var content = GetUtf8StringFromBase64(FindElement(applicationResponseXml, "Content")?.Value);

            return $"ResponseCode: {responseCode}, ResponseText: {responseText}, Content: {content}";
        }

        private static XElement GetSimpleRequestHeader(XNamespace ns, string customerId, int requestId, DateTime timestamp)
        {
            return new XElement(ns + "RequestHeader",
                new XElement(ns + "SenderId", customerId),
                new XElement(ns + "RequestId", requestId),
                new XElement(ns + "Timestamp", timestamp));
        }

        public static XElement GetCertificatesRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, Dictionary<string, XNamespace> namespaces, string transferKey)
        {
            var applicationRequest = GetCertificateApplicationRequest(customerId, timestamp, environment, softwareId, namespaces, transferKey);
            var requestBase64 = GetBase64String(applicationRequest.ToString());
            var requestHeader = GetSimpleRequestHeader(namespaces["opc"], customerId, requestId, timestamp);

            return new XElement(namespaces["opc"] + "getServiceCertificatesin",
                requestHeader,
                new XElement(namespaces["opc"] + "ApplicationRequest", requestBase64));
        }

        public static XElement GetCertificateRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, int requestId, Dictionary<string, XNamespace> namespaces, string transferKey, string pkcs10)
        {
            var applicationRequest = GetCertificateApplicationRequest(customerId, timestamp, environment, softwareId, namespaces, transferKey, pkcs10);
            var requestBase64 = GetBase64String(applicationRequest.ToString());
            var requestHeader = GetSimpleRequestHeader(namespaces["opc"], customerId, requestId, timestamp);

            return new XElement(namespaces["opc"] + "getCertificatein",
                requestHeader,
                new XElement(namespaces["opc"] + "ApplicationRequest", requestBase64));
        }

        private static XElement GetCertificateApplicationRequest(string customerId, DateTime timestamp, Environment environment, string softwareId, Dictionary<string, XNamespace> namespaces, string transferKey, string pkcs10 = null)
        {
            var properties = new List<XElement>
            {
                new XElement(namespaces["dat"] + "CustomerId", customerId),
                new XElement(namespaces["dat"] + "Timestamp", timestamp),
                new XElement(namespaces["dat"] + "Environment", environment.ToString()),
                new XElement(namespaces["dat"] + "SoftwareId", softwareId),
                new XElement(namespaces["dat"] + "Service", "MATU"),
                string.IsNullOrEmpty(pkcs10) ? null : new XElement(namespaces["dat"] + "Content", pkcs10),
                new XElement(namespaces["dat"] + "TransferKey", transferKey)
            }.Where(p => p != null);

            return new XElement(namespaces["dat"] + "CertApplicationRequest", properties);
        }

        public static XmlDocument GetEmptySoapEnvelope(XElement soapBodyContent, DateTime timestamp, Dictionary<string, XNamespace> namespaces)
        {
            var soapHeader = new XElement(namespaces["env"] + "Header");
            var soapBody = new XElement(namespaces["env"] + "Body",
                soapBodyContent);

            var result = new XElement(namespaces["env"] + "Envelope", ConvertNamespacesToAttributes(namespaces),
                                    soapHeader,
                                    soapBody);

            var soapRequestDocument = GetXmlDocument(result.ToString());

            return soapRequestDocument;
        }
    }
}