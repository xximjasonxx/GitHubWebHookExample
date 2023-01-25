
terraform {
  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.15.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "funcgithubhook"
    storage_account_name = "funcgithubhook"
    container_name       = "tfstate"
  }
  
}

# Configure the Azure Active Directory Provider
provider "azuread" {
  tenant_id = "81699cc3-1e16-40c8-afb9-5b4e2aac2dca"
  client_id = "57c6f783-454f-4b78-a4cf-e506ef1f0c1b"
  client_secret = "iTW8Q~ZPM0uoYsRxj3E6d-jNZFi3sRR~wWomidzf"
}

variable application_name {
  type = string
  description = "The name of the application"
}

variable application_roles {
  type          = list(object({
    display_name      = string
    role_name         = string
  }))
}

resource random_uuid this {
  count         = length(var.application_roles)
}

resource azuread_application this {
  display_name      = var.application_name

  dynamic "app_role" {
    for_each = var.application_roles
    content {
      allowed_member_types = ["Application"]
      description          = app_role.value.display_name
      display_name         = app_role.value.display_name
      enabled              = true
      value                = app_role.value.role_name
      id                   = random_uuid.this[index(var.application_roles, app_role.value)].result
    }
  }
}