FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Esport.Generator.Service/Esport.Generator.Service.csproj", "Esport.Generator.Service/"]
COPY ["Esport.Generator.Service.Core/Esport.Generator.Service.Core.csproj", "Esport.Generator.Service.Core/"]
COPY ["Esport.Kafka.Publisher/Esport.Kafka.Publisher.csproj", "Esport.Kafka.Publisher/"]
COPY ["Esport.Kafka.Common/Esport.Kafka.Common.csproj", "Esport.Kafka.Common/"]
RUN dotnet restore "Esport.Generator.Service/Esport.Generator.Service.csproj"
COPY . .
WORKDIR "/src/Esport.Generator.Service"
RUN dotnet build "Esport.Generator.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Esport.Generator.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Esport.Generator.Service.dll"]
