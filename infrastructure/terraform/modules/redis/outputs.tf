output "redis_hostname" {
  value = azurerm_redis_cache.this.hostname
}

output "redis_ssl_port" {
  value = azurerm_redis_cache.this.ssl_port
}

output "redis_primary_key" {
    value = azurerm_redis_cache.this.primary_access_key
}