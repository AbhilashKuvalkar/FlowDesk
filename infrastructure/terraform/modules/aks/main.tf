locals {
  aks_name   = "flowdesk-aks-${var.environment_name}"
  node_count = var.environment_name == "prd" ? 3 : 1
}

resource "azurerm_kubernetes_cluster" "this" {
  name                = local.aks_name
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = "flowdesk-${var.environment_name}"

  default_node_pool {
    name       = "system"
    node_count = local.node_count
    vm_size    = "Standard_D2s_v3"
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin    = "azure"
    load_balancer_sku = "standard"
  }

  lifecycle {
    prevent_destroy = true
  }
}

data "azurerm_key_vault" "this" {
  name                = var.key_vault_name
  resource_group_name = var.resource_group_name
}

resource "azurerm_role_assignment" "aks_kv_secrets_user" {
  scope                = data.azurerm_key_vault.this.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_kubernetes_cluster.this.kubelet_identity[0].object_id

  lifecycle {
    prevent_destroy = true
  }
}
