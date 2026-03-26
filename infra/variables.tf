terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

variable "subscription_id" {
  type    = string
  default = "" # set to your subscription ID or via TF_VAR_subscription_id
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id != "" ? var.subscription_id : null
}

variable "location" {
  type    = string
  default = "West Europe"
}

variable "resource_group_name" {
  type    = string
  default = "rg-euw-trainingsandbox"
}

variable "app_service_plan_name" {
  type    = string
  default = "barmanbank-asp"
}

variable "webapp_name" {
  type    = string
  default = "barmanbank-app-demo-001" # must be globally unique
}

variable "sql_server_name" {
  type    = string
  default = "barmanbank-sqlsrv-001" # must be globally unique
}

variable "sql_database_name" {
  type    = string
  default = "barmanbankdb"
}

variable "sql_admin_user" {
  type    = string
  default = "sqladminuser"
}

variable "sql_admin_password" {
  type    = string
  description = "Use a strong password for production. For learning, adjust as needed."
  default = "P@ssword1234!"
  sensitive = true
}

variable "app_service_sku" {
  type    = string
  default = "B1" # Basic tier for PoC
}

variable "app_service_worker_size" {
  type    = number
  default = 1
}

variable "sql_sku_name" {
  type    = string
  default = "Basic"
}

variable "sql_sku_tier" {
  type    = string
  default = "Basic"
}