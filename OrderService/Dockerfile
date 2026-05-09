# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src

# Copy project file
COPY ["OrderService/OrderService.csproj", "OrderService/"]
RUN dotnet restore "OrderService/OrderService.csproj"

# Copy source code
COPY . .
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

# Publish stage
FROM build as publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "OrderService.dll"]
