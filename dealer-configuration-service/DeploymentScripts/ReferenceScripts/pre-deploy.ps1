$Path = ".\$SqlImportSchemaScript"
Write-Host "Importing Database Schema from File: $Path into: $AzureDatabaseName"
sqlcmd -U $AzureDatabaseServerAdminLogin -P $AzureDatabaseServerAdminPassword -S "$AzureDatabaseServerName.database.windows.net" -d $AzureDatabaseName -i $Path
Write-Host "SQL Executed"