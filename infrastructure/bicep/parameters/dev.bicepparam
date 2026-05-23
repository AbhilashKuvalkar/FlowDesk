using '../main.bicep'

param environmentName = 'dev'
param sqlAdminLogin = 'flowdeskadmin'
param sqlAdminPassword = readEnvironmentVariable('SQL_ADMIN_PASSWORD')
