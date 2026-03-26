data "azurerm_resource_group" "rg" {
  name = var.resource_group_name
}

resource "azurerm_service_plan" "asp" {
  name                = var.app_service_plan_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = var.app_service_sku
}

resource "azurerm_windows_web_app" "webapp" {
  name                = var.webapp_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"               = "Production"
    "ConnectionStrings__DefaultConnection" = "Server=tcp:${azurerm_mssql_server.sql.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.db.name};Persist Security Info=False;User ID=${var.sql_admin_user};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    "WEBSITE_RUN_FROM_PACKAGE"            = "1"
  }

  depends_on = [azurerm_mssql_database.db]
}

resource "azurerm_mssql_server" "sql" {
  name                         = var.sql_server_name
  resource_group_name          = data.azurerm_resource_group.rg.name
  location                     = data.azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_user
  administrator_login_password = var.sql_admin_password
}

resource "azurerm_mssql_database" "db" {
  name      = var.sql_database_name
  server_id = azurerm_mssql_server.sql.id

  sku_name = "Basic"
}


# Optional: allow outbound to SQL Azure from the app service by default (service endpoint / firewall is typically open for Azure services)
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name                = "allow_azure_services"
  server_id           = azurerm_mssql_server.sql.id
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

# Allow local development IP
resource "azurerm_mssql_firewall_rule" "allow_local_ip" {
  name                = "AllowMyLocalIP"
  server_id           = azurerm_mssql_server.sql.id
  start_ip_address    = "139.167.154.166"
  end_ip_address      = "139.167.154.166"
}
