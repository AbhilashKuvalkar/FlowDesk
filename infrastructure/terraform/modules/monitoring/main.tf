locals {
  log_analytics_workspace_name = "flowdesk-logAnalytics-${var.environment_name}"
  application_insights_name    = "flowdesk-appInsights-${var.environment_name}"
  retentionInDays              = 30
}

resource "azurerm_log_analytics_workspace" "this" {
  name                            = local.log_analytics_workspace_name
  resource_group_name             = var.resource_group_name
  location                        = var.location
  retention_in_days               = local.retentionInDays
  local_authentication_disabled   = true
  allow_resource_only_permissions = true
  internet_ingestion_enabled      = true
  internet_query_enabled          = true
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_application_insights" "this" {
  name                          = local.application_insights_name
  resource_group_name           = var.resource_group_name
  location                      = var.location
  application_type              = "web"
  workspace_id                  = azurerm_log_analytics_workspace.this.id
  disable_ip_masking            = false
  local_authentication_disabled = false
  retention_in_days             = local.retentionInDays
  sampling_percentage           = 100
  internet_ingestion_enabled    = true
  internet_query_enabled        = true
}
