#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tv2CsGoCalendar/Tv2CsGoCalendar.csproj", "Tv2CsGoCalendar/"]
RUN dotnet restore "Tv2CsGoCalendar/Tv2CsGoCalendar.csproj"
COPY . .
WORKDIR "/src/Tv2CsGoCalendar"
RUN dotnet build "Tv2CsGoCalendar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tv2CsGoCalendar.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tv2CsGoCalendar.dll"]