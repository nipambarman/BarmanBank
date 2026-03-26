output "app_service_default_hostname" {
  value = azurerm_windows_web_app.webapp.default_hostname
}

output "app_service_url" {
  value = "https://${azurerm_windows_web_app.webapp.default_hostname}"
}

output "sql_fqdn" {
  value = azurerm_mssql_server.sql.fully_qualified_domain_name
}

output "connection_string" {
  value = azurerm_windows_web_app.webapp.app_settings["ConnectionStrings__DefaultConnection"]
  sensitive = true
}