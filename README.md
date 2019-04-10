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

### Task Parameters


| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the certificate that should be used for signing the messages |  |
| Environment | string | Target environment (TEST or PRODUCTION) | TEST |
| Customer Id | string | Unique number identifying the bank's customer | 0000000000 |
| Request Id | int | A unique integer value identifying the request | 1 |
| File Type | bool | Optional parameter for filtering filelists | pain.001.001.02 |
| Start Date| object | Optional parameter for filtering filelists. Files created after will be returned. | 2018-05-25T08:10:01.7001957+03:00 |
| End Date | object |  Optional parameter for filtering filelists. Files created before will be returned. | 2018-05-28T08:10:01.7001957+03:00 |
| Status | string |  Optional parameter for filtering filelist. Valid values for files sent to hte bank by the customer are "WFP" or "FWD" (WFP = waiting for processing. FWD = forwarded). Valid values for files sent to hte bank by the customer are "NEW" or "DLD" (NEW = files not downloaded yet. DLD = files already downloaded). If no parameter is given or if the status is "ALL", all files will be listed. | NEW |


### Result
| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| EmailSent | bool | Returns true if email message has been sent | true |
| StatusString| string | Contains information about the task's result. | No attachments found matching path \"C:\\temp\\*.csv\". No email sent. |

## UploadFile



### Task parameters


| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Url | string | URL of the bank's web service | https://wsk.asiakastesti.op.fi |
| Certificate Issued By | string | The issuer of the certificate that should be used for signing the messages |  |
| Environment | string | Target environment (TEST or PRODUCTION) | TEST |
| Customer Id | string | Unique number identifying the bank's customer | 0000000000 |
| Request Id | int | A unique integer value identifying the request | 1 |
| File Type | bool | Optional parameter for filtering filelists | pain.001.001.02 |
| Start Date| object | Optional parameter for filtering filelists. Files created after will be returned. | 2018-05-25T08:10:01.7001957+03:00 |
| End Date | object |  Optional parameter for filtering filelists. Files created before will be returned. | 2018-05-28T08:10:01.7001957+03:00 |
| Status | string |  Optional parameter for filtering filelist. Valid values for files sent to hte bank by the customer are "WFP" or "FWD" (WFP = waiting for processing. FWD = forwarded). Valid values for files sent to hte bank by the customer are "NEW" or "DLD" (NEW = files not downloaded yet. DLD = files already downloaded). If no parameter is given or if the status is "ALL", all files will be listed. | NEW |


### Result


### Usage

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
