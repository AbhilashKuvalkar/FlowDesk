locals {
  acr_name = "flowdesk-acr-${var.environment_name}"
}

resource "azurerm_container_registry" "this" {
  name                = local.acr_name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Basic"
  admin_enabled       = false
  lifecycle {
    prevent_destroy = true
  }
}
