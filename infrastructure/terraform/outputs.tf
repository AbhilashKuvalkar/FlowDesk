output "aks_name" {
  value       = module.aks.aks_name
  description = "AKS cluster name for kubectl configuration"
}

output "acr_login_name" {
  value = module
}

output "acr_login_server" {
  value       = module.acr.acr_login_server
  description = "Container registry login server URL"
}
