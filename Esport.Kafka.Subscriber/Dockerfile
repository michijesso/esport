FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Esport.Kafka.Subscriber/Esport.Kafka.Subscriber.csproj", "Esport.Kafka.Subscriber/"]
COPY ["Esport.Kafka.Common/Esport.Kafka.Common.csproj", "Esport.Kafka.Common/"]
COPY ["Esport.Domain/Esport.Domain.csproj", "Esport.Domain/"]
COPY ["Esport.Infrastructure/Esport.Infrastructure.csproj", "Esport.Infrastructure/"]
RUN dotnet restore "Esport.Kafka.Subscriber/Esport.Kafka.Subscriber.csproj"
COPY . .
WORKDIR "/src/Esport.Kafka.Subscriber"
RUN dotnet build "Esport.Kafka.Subscriber.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Esport.Kafka.Subscriber.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Esport.Kafka.Subscriber.dll"]
