output "aks_name" {
  value = azurerm_kubernetes_cluster.this.name
}

output "aks_fqdn" {
  value = azurerm_kubernetes_cluster.this.fqdn
}

output "kubelet_identity_object_id" {
  value = azurerm_kubernetes_cluster.this.kubelet_identity[0].object_id
}