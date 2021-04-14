#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OpenHim.Mediator.Hl7Validator.csproj", "OpenHim.Mediator.Hl7Validator/"]
RUN dotnet restore "OpenHim.Mediator.Hl7Validator/OpenHim.Mediator.Hl7Validator.csproj"
WORKDIR "/src/OpenHim.Mediator.Hl7Validator"
COPY . .
RUN dotnet build "OpenHim.Mediator.Hl7Validator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenHim.Mediator.Hl7Validator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenHim.Mediator.Hl7Validator.dll"]