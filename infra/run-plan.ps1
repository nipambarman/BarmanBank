#!/usr/bin/env pwsh
Set-Location "d:\BarmanBank\infra"
Write-Host "Running terraform plan..." -ForegroundColor Cyan
terraform init -input=false
terraform plan -out=tfplan
Write-Host "Plan saved to tfplan" -ForegroundColor Green
