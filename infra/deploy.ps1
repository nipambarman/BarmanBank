# Helper script for PoC deployment
# Run from d:\BarmanBank\infra

param(
    [string]$TerraformDir = "d:\BarmanBank\infra",
    [string]$PublishDir = "d:\BarmanBank\publish",
    [string]$ZipFile = "d:\BarmanBank\publish.zip"
)

# Build the app
cd "d:\BarmanBank"
Write-Host "Publishing app to $PublishDir"
dotnet publish -c Release -o $PublishDir

# Package
Write-Host "Creating ZIP artifact"
if (Test-Path $ZipFile) { Remove-Item $ZipFile }
Compress-Archive -Path "$PublishDir\*" -DestinationPath $ZipFile

# Terraform
cd $TerraformDir
terraform init
terraform apply -auto-approve

# Deploy to App Service
$webapp = (terraform output -raw app_service_name 2>$null)
if (-not $webapp) { $webapp = "barmanbank-app-demo-001" }
$rg = (terraform output -raw resource_group_name 2>$null)
if (-not $rg) { $rg = "barmanbank-rg" }

Write-Host "Deploying ZIP to Azure Web App $webapp"
az webapp deployment source config-zip --resource-group $rg --name $webapp --src $ZipFile

Write-Host "Deployment complete. App URL: $(terraform output -raw app_service_url)"