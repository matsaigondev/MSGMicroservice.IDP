FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MSGMicroservice.IDP/MSGMicroservice.IDP.csproj", "MSGMicroservice.IDP/"]
RUN dotnet restore "MSGMicroservice.IDP.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MSGMicroservice.IDP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSGMicroservice.IDP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSGMicroservice.IDP.dll"]
