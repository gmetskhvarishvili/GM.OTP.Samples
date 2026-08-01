FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GM.OTP.Sample.API/GM.OTP.Sample.API.csproj", "GM.OTP.Sample.API/"]
RUN dotnet restore "GM.OTP.Sample.API/GM.OTP.Sample.API.csproj"
COPY . .
WORKDIR "/src/GM.OTP.Sample.API"
RUN dotnet build "GM.OTP.Sample.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GM.OTP.Sample.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GM.OTP.Sample.API.dll"]
