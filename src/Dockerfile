#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OpenHim.Mediator.HL7Validator/OpenHim.Mediator.HL7Validator.csproj", "OpenHim.Mediator.HL7Validator/"]
COPY ["WcPhdc.OpenHim.Mediator/WcPhdc.OpenHim.Mediator.csproj", "WcPhdc.OpenHim.Mediator/"]
RUN dotnet restore "OpenHim.Mediator.HL7Validator/OpenHim.Mediator.HL7Validator.csproj"
COPY . .
WORKDIR "/src/OpenHim.Mediator.HL7Validator"
RUN dotnet build "OpenHim.Mediator.HL7Validator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenHim.Mediator.HL7Validator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenHim.Mediator.HL7Validator.dll"]