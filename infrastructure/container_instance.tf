terraform {
	required_providers {
		azurerm = {
			source = "hashicorp/azurerm"
			version = ">=2.32"
		}
	}
}

provider "azurerm" {
	skip_provider_registration = true
	subscription_id = "b44ed40d-3b2d-499b-9c38-4ee363ca9811"

	features {}
}

resource "azurerm_container_group" "hl7_validator_cg" {
  name                = "hl7-validator-cg"
  location            = "uksouth"
  resource_group_name = "RG-NONPROD-PHDC-ZAN"
  ip_address_type     = "public"
  dns_name_label      = "hl7-validator"
  os_type             = "Linux"

  container {
    name   = "hl7-validator"
    image  = "index.docker.io/mornemaritz/hl7-validator:0.1.0"
    cpu    = "0.5"
    memory = "1.5"
	
	environment_variables = {
		"mediatorconfig__mediatorCore__heartbeatEnabled" 	= false
	}
	
	secure_environment_variables = {
		"mediatorconfig__openHimAuth__corePassword" = file("~/.openhim_core_pw")
		"mediatorconfig__openHimAuth__apiClientPassword" = file("~/.openhim_api_client_pw")
	}
	
    ports {
      port     = 80
      protocol = "TCP"
    }
  }
}

