variable "location" { type = string }

variable "environment_name" { type = string }

variable "resource_group_name" { type = string }

variable "sql_connection_string" {
  type      = string
  sensitive = true
  default   = ""
}

variable "redis_hostname" { type = string }

variable "redis_ssl_port" { type = number }

variable "redis_primary_key" { type = string }

variable "app_insights_connection_string" {
  type      = string
  sensitive = true
  default   = ""
}

variable "app_insights_instrumentation_key" {
  type      = string
  sensitive = true
  default   = ""
}
