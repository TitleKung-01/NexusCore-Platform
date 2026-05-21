FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["NexusCore.Api/NexusCore.Api.csproj", "NexusCore.Api/"]
COPY ["NexusCore.Application/NexusCore.Application.csproj", "NexusCore.Application/"]
COPY ["NexusCore.Domain/NexusCore.Domain.csproj", "NexusCore.Domain/"]
COPY ["NexusCore.Infrastructure/NexusCore.Infrastructure.csproj", "NexusCore.Infrastructure/"]
RUN dotnet restore "NexusCore.Api/NexusCore.Api.csproj"
COPY . .
RUN dotnet publish "NexusCore.Api/NexusCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5100
ENV ASPNETCORE_URLS=http://+:5100
ENTRYPOINT ["dotnet", "NexusCore.Api.dll"]
