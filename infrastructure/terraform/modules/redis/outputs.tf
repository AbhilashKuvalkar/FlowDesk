output "redis_hostname" {
  value = azurerm_managed_redis.this.hostname
}

output "redis_ssl_port" {
  value = azurerm_managed_redis.this.default_database[0].port
}

output "redis_primary_key" {
  value = azurerm_managed_redis.this.default_database[0].primary_access_key
}
