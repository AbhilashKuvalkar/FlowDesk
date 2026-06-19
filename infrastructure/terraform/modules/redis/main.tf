locals {
  redis_name = "flowdesk-redis-${var.environment_name}"
}

resource "azurerm_managed_redis" "this" {
  name                  = local.redis_name
  resource_group_name   = var.resource_group_name
  location              = var.location
  sku_name              = "Balanced_B50"
  public_network_access = "Disabled"
  default_database {
    access_keys_authentication_enabled = true
  }

  identity {
    type = "SystemAssigned"
  }

  lifecycle {
    prevent_destroy = true
  }
}

