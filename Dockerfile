# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies with full clean
COPY TripNest.csproj ./
RUN dotnet nuget locals all --clear
RUN dotnet restore TripNest.csproj --force --no-cache --verbosity normal

# Copy everything else and build the app
COPY . ./
RUN dotnet publish TripNest.csproj -c Release -o out --no-restore --verbosity normal

# Use the official ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app from build stage
COPY --from=build /app/out .

# Expose port 8080 (Render requirement)
EXPOSE 8080

# Set environment variable for ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the app
ENTRYPOINT ["dotnet", "TripNest.dll"]