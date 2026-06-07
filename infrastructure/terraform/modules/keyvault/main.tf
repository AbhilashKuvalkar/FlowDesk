locals {
  key_vault_name = "flowdesk-kv-${var.environment_name}"
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "this" {
  name                       = local.key_vault_name
  resource_group_name        = var.resource_group_name
  location                   = var.location
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  rbac_authorization_enabled = true
  soft_delete_retention_days = 7
  purge_protection_enabled   = true
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_key_vault_secret" "sql_connection_string" {
  name         = "SqlConnectionString"
  key_vault_id = azurerm_key_vault.this.id
  value        = var.sql_connection_string
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_key_vault_secret" "redis_connection_string" {
  name         = "RedisConnectionString"
  key_vault_id = azurerm_key_vault.this.id
  value        = "${var.redis_hostname}:${var.redis_ssl_port},password=${var.redis_primary_key},ssl=True"
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_role_assignment" "terraform_kv_admin" {
  scope                = azurerm_key_vault.this.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = data.azurerm_client_config.current.object_id
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_key_vault_secret" "app_insights_connection_string" {
  name         = "AppInsightsConnectionString"
  value        = var.app_insights_connection_string
  key_vault_id = azurerm_key_vault.this.id
  depends_on   = [azurerm_role_assignment.terraform_kv_admin]
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_key_vault_secret" "app_insights_instrumentation_key" {
  name         = "AppInsightsInstrumentationKey"
  value        = var.app_insights_instrumentation_key
  key_vault_id = azurerm_key_vault.this.id
  depends_on   = [azurerm_role_assignment.terraform_kv_admin]
  lifecycle {
    prevent_destroy = true
  }
}
