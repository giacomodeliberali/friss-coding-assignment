FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY ["src/Web.Host/Web.Host.csproj", "Web.Host/"]
COPY ["src/Web/Web.csproj", "Web/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Application.Contracts/Application.Contracts.csproj", "Application.Contracts/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/EntityFrameworkCore/EntityFrameworkCore.csproj", "EntityFrameworkCore/"]
COPY ["src/WriteModel/WriteModel.csproj", "WriteModel/"]
COPY ["test/UnitTests/UnitTests.csproj", "UnitTests/"]
COPY ["test/IntegrationTests/IntegrationTests.csproj", "IntegrationTests/"]

RUN dotnet restore "Web.Host/Web.Host.csproj"

COPY . .

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.Host.dll"]
