@echo off
cd /d d:\BarmanBank\infra
for /f "tokens=*" %%i in ('terraform output -raw connection_string') do set connStr=%%i
cd /d d:\BarmanBank
dotnet ef database update --connection "%connStr%"
echo Migration completed!