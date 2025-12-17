# Usa la imagen oficial de .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo del proyecto y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia todo el código y compila
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Imagen final más pequeña
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expone el puerto que usa tu app (normalmente 80 o 5000)
EXPOSE 80
ENTRYPOINT ["dotnet", "PresupuestoMVC.dll"]
