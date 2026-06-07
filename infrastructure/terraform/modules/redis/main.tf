locals {
  redis_name = "flowdesk-redis-${var.environment_name}"
}

resource "azurerm_redis_cache" "this" {
  name                 = local.redis_name
  resource_group_name  = var.resource_group_name
  location             = var.location
  sku_name             = "Basic"
  capacity             = 0
  family               = "C"
  minimum_tls_version  = "1.2"
  non_ssl_port_enabled = false

  redis_configuration {
    maxmemory_policy = "allkeys-lru"
  }

  lifecycle {
    prevent_destroy = true
  }
}

