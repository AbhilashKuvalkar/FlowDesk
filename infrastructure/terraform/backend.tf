terraform {
  required_version = ">= 1.6.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.77"
    }
  }

  backend "azurerm" {
    resource_group_name  = "flowdesk-tfstate-rg"
    storage_account_name = "flowdesktfstate"
    container_name       = "tfstate"
    key                  = "flowdesk.tfstate"
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}
