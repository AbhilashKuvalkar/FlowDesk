output "sql_server_fqdn" {
  value = azurerm_mssql_server.this.fully_qualified_domain_name
}

output "connection_string" {
  value     = "Server=${azurerm_mssql_server.this.fully_qualified_domain_name};Database=${local.database_name};User Id=${var.sql_admin_login};Password=${var.sql_admin_password};Encrypt=True;TrustServerCertificate=False;"
  sensitive = true
}
