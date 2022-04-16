#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IncursionWebhook.csproj", "."]
RUN dotnet restore "./IncursionWebhook.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "IncursionWebhook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IncursionWebhook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IncursionWebhook.dll"]