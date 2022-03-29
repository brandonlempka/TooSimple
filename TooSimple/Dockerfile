#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["TooSimple/TooSimple.csproj", "TooSimple/"]
COPY ["TooSimple.ApiControllers/TooSimple.ApiControllers.csproj", "TooSimple.ApiControllers/"]
COPY ["TooSimple.DataAccessors/TooSimple.DataAccessors.csproj", "TooSimple.DataAccessors/"]
COPY ["TooSimple.Managers/TooSimple.Managers.csproj", "TooSimple.Managers/"]
COPY ["TooSimple.Poco/TooSimple.Poco.csproj", "TooSimple.Poco/"]
RUN dotnet restore "TooSimple/TooSimple.csproj"
COPY . .
WORKDIR "/src/TooSimple"
RUN apt-get update && apt-get install -y nodejs
RUN dotnet build "TooSimple.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TooSimple.csproj" -c Release -o /app/publish -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TooSimple.dll"]