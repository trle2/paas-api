FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../SampleWebApp/SampleWebApp.csproj", "SampleWebApp/"]
RUN dotnet restore "../SampleWebApp/SampleWebApp.csproj"
COPY . .
RUN dotnet build "../SampleWebApp/SampleWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "../SampleWebApp/SampleWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleWebApp.dll"]