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

Read emails from Exchange or IMAP server.


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
ReadEmail task returns a list of EmailMessageResult objects. Each object contains following properties:

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|Id                         |string                     |Email message id           | ... |
|To                         |string                     |To field from email        |agent@frends.com|
|Cc                         |string                     |Cc field from email        |doubleagent@frends.com|
|From                       |string                     |From field from email      |sender@frends.com|
|Date                       |DateTime                   |Received date              | ... |
|Subject                    |string                     |Email subject              |Important email!|
|BodyText                   |string                     |Plain text email body      | ... |
|BodyHtml                   |string                     |Html email body            | ... |

### Usage
You can loop email message by giving task result as input to foreach-shape:
```sh
#result[ReadEmail]
```

You can reference email properties like so:
```sh
#result[ReadEmail][0].BodyText
```
## FetchExchangeAttachments

Fetches attachments from an Exchange server.

### Server settings

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|ExchangeServerVersion      |enum                       |Exchange server version    |Exchange2013_SP1|
|UseAutoDiscover            |bool                       |If true, task will try to autodiscover exchange server address from given email address|true|
|ServerAddress              |string                     |Exchange server address    |exchange.frends.com|
|UseAgentAccount            |bool                       |If true, will try to authenticate against server with the running frends agent account|false|
|EmailAddress               |string                     |Account email address      |agent@frends.com|
|Password                   |string                     |Account password           |***|

### Options

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|MaxEmails                  |int                        |Maximum number of emails to retrieve|10|
|AttachmentSaveDirectory    |string                     |Directory where attachments will be saved to.|C:\WorkDir\|
|OverwriteAttachment        |bool                       |If true, files in the save directory with the sama name as the attachment will be overwritten|false|
|EmailSenderFilter          |string                     |Optional. If a sender is given, it will be used to filter emails.|sender@frends.com|
|EmailSubjectFilter         |string                     |Optional. If a subject is given, it will be used to filter emails (match as substring).|Payments|
|ThrowErrorIfEmailNotFound  |bool                       |If true, error will be thrown if no attachments are found|false|
|GetOnlyUnreadEmails        |bool                       |If true, only attachments of unread emails will be fetched|false|
|MarkEmailsAsRead           |bool                       |If true, will mark processed emails as read (unless execution is cancelled during processing) |false|
|DeleteReadEmails           |bool                       |If true, will delete processed emails from server (unless execution is cancelled during processing)|false|

### Result
FetchExchangeAttachments task returns a list of EmailAttachmentResult objects. Each object contains following properties:

|Property                   |Type                       |Description                     |Example|
|---------------------------|---------------------------|--------------------------------|---------------|
|Id                         |string                     |Email message id                | ... |
|To                         |string                     |To field from email             |agent@frends.com|
|Cc                         |string                     |Cc field from email             |doubleagent@frends.com|
|From                       |string                     |From field from email           |sender@frends.com|
|Date                       |DateTime                   |Received date                   | ... |
|Subject                    |string                     |Email subject                   |Important email!|
|BodyText                   |string                     |Plain text email body           | ... |
|AttachmentSaveDirs         |List of strings            |Full paths to saved attachments | {"C:\WorkDir\attchmnt1.txt","C:\WorkDir\attchmnt2.txt"}  |

### Usage
You can loop resulting objects by giving task result as input to foreach-shape:
```sh
#result[FetchExchangeAttachments]
```

You can reference result properties like so:
```sh
#result[FetchExchangeAttachments][0].BodyText
```

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.Email.git`

Restore dependencies

`nuget restore frends.community.email`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.Email.Tests\bin\Release\Frends.Community.Email.Tests.dll`

Create a nuget package

`nuget pack nuspec/Frends.Community.Email.nuspec`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial version of SendEmail |
| 1.1.23 | Added FetchExchangeAttachment |
