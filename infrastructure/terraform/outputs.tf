output "aks_name" {
  value       = module.aks.aks_name
  description = "AKS cluster name for kubectl configuration"
}

output "acr_login_server" {
  value       = module.acr.acr_login_server
  description = "Container registry login server URL"
}

output "key_vault_uri" {
  value       = module.keyvault.key_vault_uri
  description = "Key Vault URI for application configuration"
}