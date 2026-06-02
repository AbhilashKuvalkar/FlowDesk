resource "azurerm_resource_group" "this" {
  name     = "flowdesk-${var.environment_name}-rg"
  location = var.location
}

module "sql" {
  source              = "./modules/sql"
  location            = var.location
  environment_name    = var.environment_name
  resource_group_name = azurerm_resource_group.this.name
  sql_admin_login     = var.sql_admin_login
  sql_admin_password  = var.sql_admin_password
}

module "redis" {
  source              = "./modules/redis"
  location            = var.location
  environment_name    = var.environment_name
  resource_group_name = azurerm_resource_group.this.name
}

module "acr" {
  source              = "./modules/acr"
  location            = var.location
  environment_name    = var.environment_name
  resource_group_name = azurerm_resource_group.this.name
}

module "monitoring" {
  source              = "./modules/monitoring"
  location            = var.location
  environment_name    = var.environment_name
  resource_group_name = azurerm_resource_group.this.name
}

module "keyvault" {
  source                           = "./modules/keyvault"
  location                         = var.location
  environment_name                 = var.environment_name
  resource_group_name              = azurerm_resource_group.this.name
  sql_connection_string            = module.sql.connection_string
  redis_hostname                   = module.redis.redis_hostname
  redis_ssl_port                   = module.redis.redis_ssl_port
  redis_primary_key                = module.redis.redis_primary_key
  app_insights_connection_string   = module.monitoring.app_insights_connection_string
  app_insights_instrumentation_key = module.monitoring.app_insights_instrumentation_key
}

module "aks" {
  source              = "./modules/aks"
  location            = var.location
  environment_name    = var.environment_name
  resource_group_name = azurerm_resource_group.this.name
  key_vault_name      = module.keyvault.key_vault_name

  depends_on = [module.keyvault]
}
