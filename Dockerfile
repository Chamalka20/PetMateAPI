# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.sln ./
COPY PetMateAPI/*.csproj ./PetMateAPI/

RUN dotnet restore

COPY . ./
RUN dotnet build -c Release -o out

# Use .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "PetMateAPI.dll"]