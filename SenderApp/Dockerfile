# Stage 1: Base runtime image
# ----------------------------
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# Stage 2: Build app
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SenderApp/SenderApp.csproj", "SenderApp/"]
COPY ["MessagingConsoleLib/MessagingConsoleLib.csproj", "MessagingConsoleLib/"]
RUN dotnet restore "./SenderApp/SenderApp.csproj"
COPY . .
WORKDIR "/src/SenderApp"
RUN dotnet build "./SenderApp.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Stage 3: Publish app
# ----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SenderApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Stage 4: Final runtime image
# ----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SenderApp.dll"]