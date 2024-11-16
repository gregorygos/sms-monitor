# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# COPY
COPY *.csproj ./
RUN dotnet restore
COPY . .

# PUBLISH
RUN dotnet publish -c Release -o /app

# RUN
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

# EXPOSE
EXPOSE 4000

ENTRYPOINT ["dotnet", "SmsRelay.dll"]

