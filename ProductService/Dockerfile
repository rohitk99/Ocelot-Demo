# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src

# Copy project file
COPY ["ProductService/ProductService.csproj", "ProductService/"]
RUN dotnet restore "ProductService/ProductService.csproj"

# Copy source code
COPY . .
WORKDIR "/src/ProductService"
RUN dotnet build "ProductService.csproj" -c Release -o /app/build

# Publish stage
FROM build as publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductService.dll"]
