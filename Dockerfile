FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . ./

# Find and restore the first .csproj file
RUN dotnet restore $(find . -name "*.csproj" -not -path "*/obj/*" | head -1)

# Find and publish the first .csproj file
RUN dotnet publish $(find . -name "*.csproj" -not -path "*/obj/*" | head -1) -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE $PORT

# Auto-detect DLL name and run
CMD dotnet $(ls *.dll | head -1)