FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["src/OzonEdu.MerchandiseService.Grpc/OzonEdu.MerchandiseService.Grpc.csproj", "OzonEdu.MerchandiseService.Grpc/"]
COPY ["src/OzonEdu.MerchandiseService.HttpModels/OzonEdu.MerchandiseService.HttpModels.csproj", "OzonEdu.MerchandiseService.HttpModels/"]
COPY ["src/OzonEdu.MerchandiseService.Infrastructure/OzonEdu.MerchandiseService.Infrastructure.csproj", "OzonEdu.MerchandiseService.Infrastructure/"]
COPY ["src/OzonEdu.MerchandiseService.Domain/OzonEdu.MerchandiseService.Domain.csproj", "OzonEdu.MerchandiseService.Domain/"]
COPY ["src/OzonEdu.MerchandiseService/OzonEdu.MerchandiseService.csproj", "OzonEdu.MerchandiseService/"]
RUN dotnet restore "OzonEdu.MerchandiseService/OzonEdu.MerchandiseService.csproj"
COPY "src/OzonEdu.MerchandiseService.Grpc/." "OzonEdu.MerchandiseService.Grpc/"
COPY "src/OzonEdu.MerchandiseService.HttpModels/." "OzonEdu.MerchandiseService.HttpModels/"
COPY "src/OzonEdu.MerchandiseService.Infrastructure/." "OzonEdu.MerchandiseService.Infrastructure/"
COPY "src/OzonEdu.MerchandiseService.Domain/." "OzonEdu.MerchandiseService.Domain/"
COPY "src/OzonEdu.MerchandiseService/." "OzonEdu.MerchandiseService/"
WORKDIR "/src/OzonEdu.MerchandiseService"
RUN dotnet build "OzonEdu.MerchandiseService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OzonEdu.MerchandiseService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OzonEdu.MerchandiseService.dll"]
