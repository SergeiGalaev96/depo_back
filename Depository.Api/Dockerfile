#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Depository.Api/Depository.Api.csproj", "Depository.Api/"]
COPY ["Depository.MassTransit/Depository.MassTransitQueue.csproj", "Depository.MassTransit/"]
COPY ["Depository.Core/Depository.Core.csproj", "Depository.Core/"]
COPY ["Depository.DAL/Depository.DAL.csproj", "Depository.DAL/"]
COPY ["Depository.Mail/Depository.Mail.csproj", "Depository.Mail/"]
COPY ["Depository.Domain/Depository.Domain.csproj", "Depository.Domain/"]
RUN dotnet restore "Depository.Api/Depository.Api.csproj"
COPY . .
WORKDIR "/src/Depository.Api"
RUN dotnet build "Depository.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Depository.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Depository.Api.dll"]