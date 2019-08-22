FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["./CCA.Services.RepositoryNook.csproj", "./CCA.Services.RepositoryNook/"]
RUN dotnet restore "CCA.Services.RepositoryNook/CCA.Services.RepositoryNook.csproj"
COPY . ./CCA.Services.RepositoryNook
WORKDIR "/src/CCA.Services.RepositoryNook"
RUN dotnet build "CCA.Services.RepositoryNook.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CCA.Services.RepositoryNook.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "cca.services.repositorynook.dll"]
