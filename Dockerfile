FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

WORKDIR /app

# Copy solution
COPY ./*.sln  ./

# Copy the main source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Restore dependencies
RUN dotnet restore src/CaravelTemplate.WebApi

# Copy everything else and build
COPY . .

# Publish application in Release
WORKDIR /app/src/CaravelTemplate.WebApi

RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

WORKDIR /app

COPY --from=builder /app/src/CaravelTemplate.WebApi/dist .

ENTRYPOINT ["dotnet", "CaravelTemplate.WebApi.dll"]