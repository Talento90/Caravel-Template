FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder

WORKDIR /app

# Copy solution
COPY ./*.sln  ./

# Copy the main source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Copy the test project files
COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

# Restore dependencies
RUN dotnet restore --configfile NuGet.Config

# Copy everything else and build
COPY . .

# Publish application in Release
WORKDIR /app/src/CaravelTemplate.WebApi
RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

WORKDIR /app

COPY --from=builder /app/src/CaravelTemplate.WebApi/dist .

ENTRYPOINT ["dotnet", "CaravelTemplate.WebApi.dll"]