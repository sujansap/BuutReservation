# Use the official .NET SDK as the build image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the entire source code into the container
COPY . /src

# intall the dotnet ef tool
RUN dotnet tool install --global dotnet-ef
# voegt omgeving toe voor de dotnet ef tool
ENV PATH="$PATH:/root/.dotnet/tools"
# Bouw het project
RUN dotnet build "/src/Rise.Server/Rise.Server.csproj" -c Release 

# Bundle the migrations voor de database te maken/ aan te passen
RUN dotnet ef migrations bundle -o /app/migrations --project /src/Rise.Persistence --startup-project /src/Rise.Server --verbose --self-contained --no-build --configuration Release

# de build image
FROM build AS publish

# Publish the application
RUN dotnet publish "/src/Rise.Server/Rise.Server.csproj" -c Release -o /app/publish --no-build 
# maak een certificaat aan voor https key en pulbic key
RUN dotnet dev-certs https --export-path /app/publish/certificate --no-password --format PEM

# Build the final image using the base image and the published output
FROM base AS final
# koppieer alle nodige files die nodig zijn naar de final image.
COPY --from=publish /app/publish .
COPY --from=publish /app/migrations .
COPY --from=publish /src/dockerrunner.sh .
# geef de rechten aan app om uit te voeren
RUN chown -R app:app /app/*
# wissel van root naar user voor safety reasens
USER app
# statische env voor https certificaten mee te geven voor te runnen.
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/app/certificate"
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath="/app/certificate.key"
# container start vanuit de /app map
WORKDIR /app

# Set the entry point to run the bash script
ENTRYPOINT ["bash","/app/dockerrunner.sh"]
