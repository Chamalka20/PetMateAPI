# Use official .NET 7 SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set working directory
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY PetMateAPI/*.csproj ./PetMateAPI/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . ./

# Build the project
RUN dotnet build -c Release -o out

# Apply EF Core migrations
RUN dotnet ef database update --project PetMateAPI

# Use runtime image for smaller final container
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy built files from the build stage
COPY --from=build /app/out ./

# Expose default port
EXPOSE 5000
EXPOSE 5001

# Run the application
ENTRYPOINT ["dotnet", "PetMateAPI.dll"]