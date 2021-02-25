FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000 

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["backend-labo02-webapi.csproj", "./"]
RUN dotnet restore "backend-labo02-webapi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "backend-labo02-webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend-labo02-webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend-labo02-webapi.dll"]
