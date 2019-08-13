# Frends.Community.PaymentServices.OP
FRENDS task for using payments services of OP bank. With the task it's possible to fetch filelists, download files, upload files and delete files.

- [Installing](#installing)
- [Tasks](#tasks)
  - [Download File List](#downloadfilelist)
  - [Upload File](#uploadfile)
  - [Download File](#downloadfile)
  - [Delete File](#deletefile)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing
You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'Nuget feed coming at later date'

Tasks
=====

## DownloadFileList

Downloads a list of files available for download from OP.

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service. | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the Base-64 encoded X.509 certificate that should be used for signing the messages. |  |
| Environment | string | Target environment (TEST or PRODUCTION). | TEST |
| Customer Id | string | Unique number identifying the bank's customer. | 0000000000 |
| Request Id | int | A unique integer value identifying the request. This value must be unique for three months. | 1 |
| File Type | bool | Optional parameter for filtering filelists. | pain.001.001.02 |
| Start Date| object | Format: YYYY-MM-DD. Optional parameter for filtering filelists. Files created after will be returned. | 2018-05-25 |
| End Date | object | Format: YYYY-MM-DD. Optional parameter for filtering filelists. Files created before will be returned. | 2018-05-28 |
| Status | string |  Optional parameter for filtering filelist. Valid values for files sent to the bank are "WFP" (waiting for processing) or "FWD" (forwarded). Valid values for files provided by the bank are "NEW" (not yet downloaded) or "DLD" (already downloaded). Parameter "ALL" fetches all available files. If no parameter is given, "ALL" will be used. | NEW |
| Connection timeout seconds | int | Timeout in seconds to be used for the connection and operation. | 30 |

### Result

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
|  | JToken array | Array elements have the following properties: FileReference, TargetId, ParentFileReference, FileType, FileTimestamp, Status  |  |

## UploadFile

Uploads a file to OP.

### Task parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service. | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the Base-64 encoded X.509 certificate that should be used for signing the messages. |  |
| Environment | string | Target environment (TEST or PRODUCTION). | TEST |
| Customer Id | string | Unique number identifying the bank's customer. | 0000000000 |
| Request Id | int | A unique integer value identifying the request. This value must be unique for three months. | 1 |
| File Input | string | File input (e.g. XML content of file). |  |
| File Type | bool | File type to upload. | pain.001.001.02 |
| File Encoding | bool | File encoding for the input file. | utf-8 |
| Connection timeout seconds | int | Timeout in seconds to be used for the connection and operation. | 30 |

### Result

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
|  | JToken array | Array elements have the following properties: FileReference, TargetId, ParentFileReference, FileType, FileTimestamp, Status |  |

## DownloadFile

Downloads a file from OP. This operation requires a file reference value that is obtainable using DownLoadFileList.

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service. | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the Base-64 encoded X.509 certificate that should be used for signing the messages. |  |
| Environment | string | Target environment (TEST or PRODUCTION). | TEST |
| Customer Id | string | Unique number identifying the bank's customer. | 0000000000 |
| Request Id | int | A unique integer value identifying the request. This value must be unique for three months. | 1 |
| File Reference | string | File reference id for the file to be downloaded. | 123456 |
| File Encoding | string | Encoding of the file to be download, if empty UTF-8 will be used. | utf-8 |
| Connection timeout seconds | int | Timeout in seconds to be used for the connection and operation. | 30 |

### Result

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
|  | string | File content in string format |  |

## DeleteFile

Deletes a file from OP. Can be used for example to cancel a file upload, but only if the file has not been processed yet.
Note: This task has not yet been tested.

### Task parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service. | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the Base-64 encoded X.509 certificate that should be used for signing the messages. |  |
| Environment | string | Target environment (TEST or PRODUCTION). | TEST |
| Customer Id | string | Unique number identifying the bank's customer. | 0000000000 |
| Request Id | int | A unique integer value identifying the request. This value must be unique for three months. | 1 |
| File Reference | string | File reference id for the file to be downloaded. | 123456 |
| Connection timeout seconds | int | Timeout in seconds to be used for the connection and operation. | 30 |

### Result

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
|  | string | Returns the content of the ApplicationResponse element of the web service response |  |

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.PaymentServices.OP.git`

Restore dependencies

`nuget restore frends.community.paymentservices.op`

Rebuild the project

Create a nuget package

`nuget pack nuspec/Frends.Community.PaymentServices.OP.nuspec`

# Testing

Testing and using the tasks requires a working connection to OP's Web Service. In order to connect to OP's Web Service the following things are needed:

- Both Corporate Bank Connection Agreement and a C2B Agreement with OP-Pohjola Group. Without these there is no access to Web Services Channel, no Customer Id and no way to obtain the needed certificate.
- A client specific certificate. Test and production environments require their own certificates. Instructions on obtaining a certificate can be found from OP's web page. The certificate should be installed under Personal/Certificates in the FRENDS agent's certificate store.
- The following Entrust CA certificates: Entrust Root Certificate Authority - G2, Entrust Certificate Authority - L1K (Non-EV SSL)  

When the certificate is obtained and installed, and the needed ID values are known, the connection to OP's web service can be tested with the task DownloadFileList, as it only fetches information and does not modify anything at OP's end (except for the file status from NEW to DLD). Assuming there is some material to test with, DownLoadFile and UploadFile can be tested as soon as the connection to the web service works.

When testing - and especially when testing UploadFile - care should be taken that the environment parameter is TEST, as there is no separate test environment for the web service. If, for example, test material is uploaded with environment parameter PRODUCTION, the material will be processed in production. 


# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version              | Changes                 |
| ---------------------| ----------------------- |
| 1.0.0 | Initial version of tasks |
| 1.0.1 | Changed return type of tasks |
| 1.1.0 | A few minor fixes to code and updated documentation | 