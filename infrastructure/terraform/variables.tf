variable "environments" {
  type    = list(string)
  default = ["dev", "tst", "acc", "prd"]
}

variable "environment_name" {
  type        = string
  description = "Deployment environment (${join(", ", var.environments)})"
  validation {
    condition     = contains(var.environments, var.environment_name)
    error_message = "environment_name must be ${join(", ", var.environments)}"
  }
}

variable "location" {
  type        = string
  description = "Azure region"
  default     = "southindia"
}

variable "sql_admin_login" {
  type = string
  description = "SQL Server administrator username"
  sensitive = true
}

variable "sql_admin_password" {
  type = string
  description = "SQL Server administrator password"
  sensitive = true
}

