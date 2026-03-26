@echo off
cd /d d:\BarmanBank
rmdir /s /q .git 2>nul
git init
git remote add origin https://github.com/nipambarman/BarmanBank.git
git add .
git commit -m "Initial commit with BarmanBank app and Azure infrastructure"
git branch -M main
git push -u origin main --force
echo Git push completed successfully!