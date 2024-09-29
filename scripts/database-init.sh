#!/bin/bash

dotnet build ./CaravelTemplate.sln

# Create Database Schema Changes
#echo "Create Database Schema"
dotnet ef migrations add CreateQuartzSchema --startup-project src/CaravelTemplate.Migrator --output-dir Migrations --project src/CaravelTemplate.Adapter.Quartz --context QuartzDbContext --no-build
dotnet ef migrations add CreateApplicationSchema --startup-project src/CaravelTemplate.Migrator --output-dir Migrations --project src/CaravelTemplate.Adapter.PostgreSql --context ApplicationDbContext --no-build
dotnet ef migrations add CreateIdentitySchema --startup-project src/CaravelTemplate.Migrator --output-dir Migrations --project src/CaravelTemplate.Adapter.Identity --context IdentityDbContext --no-build

# Apply Database Schema Changes
echo "Apply Database Changes"
dotnet ef database update --startup-project src/CaravelTemplate.Migrator --project src/CaravelTemplate.Adapter.Quartz --context QuartzDbContext --no-build
dotnet ef database update --startup-project src/CaravelTemplate.Migrator --project src/CaravelTemplate.Adapter.PostgreSql --context ApplicationDbContext --no-build
dotnet ef database update --startup-project src/CaravelTemplate.Migrator --project src/CaravelTemplate.Adapter.Identity --context IdentityDbContext --no-build