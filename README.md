##Usage

###Provider Types

This API supports three provider types: SMB, AWS S3, and Azure Blob Storage. Each provider type can be specified using the providerType parameter in the API endpoints.

1. SMB
   Provider Type: SMB
   Example: Create a folder in SMB
   POST /api/files/SMB/folders?folderName=folderName

2. Azure Blob Storage
   Provider Type: Azure
   Example: Example: Create a folder in Azure
   POST /api/files/Azure/folders?folderName=folderName
