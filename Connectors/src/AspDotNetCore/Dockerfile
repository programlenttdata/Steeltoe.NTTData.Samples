FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SqlServerEFCore.csproj", "SqlServerEFCore/"]
RUN dotnet restore "SqlServerEFCore/SqlServerEFCore.csproj"
COPY . .
WORKDIR "/src/SqlServerEFCore"
RUN dotnet build "SqlServerEFCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SqlServerEFCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SqlServerEFCore.dll"]