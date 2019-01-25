FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["CCA.Services.repositorynook/CCA.Services.repositorynook.csproj", "CCA.Services.repositorynook/"]
RUN dotnet restore "CCA.Services.repositorynook/CCA.Services.repositorynook.csproj"
COPY . .
WORKDIR "/src/CCA.Services.repositorynook"
RUN dotnet build "CCA.Services.repositorynook.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CCA.Services.repositorynook.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "cca.services.repositorynook.dll"]