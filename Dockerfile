# -----------------------
# 1. Build stage
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY PetMateAPI/*.csproj ./PetMateAPI/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . ./

# Build and publish
WORKDIR /src/PetMateAPI
RUN dotnet publish -c Release -o /app/out

# -----------------------
# 2. Runtime stage
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/out ./

# Set Railway port
ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}

# Start the app
ENTRYPOINT ["dotnet", "PetMateAPI.dll"]