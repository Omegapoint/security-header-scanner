FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-composite AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js
RUN apk add --update nodejs npm

WORKDIR /src
COPY . .
RUN dotnet publish headers.security.Api -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "headers.security.Api.dll"]
