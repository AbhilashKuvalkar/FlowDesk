locals {
  sql_server_name = "flowdesk-sql-${var.environment_name}"
  database_name   = "FlowDesk"
}

resource "azurerm_mssql_server" "this" {
  name                         = local.sql_server_name
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_mssql_database" "this" {
  name      = local.database_name
  server_id = azurerm_mssql_server.this.id
  sku_name  = "Basic"
  collation = "SQL_Latin1_General_CP1_CI_AS"
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "allow_azure_services"
  server_id        = azurerm_mssql_server.this.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
  lifecycle {
    prevent_destroy = true
  }
}
