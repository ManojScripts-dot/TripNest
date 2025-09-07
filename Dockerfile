FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app
COPY . ./
RUN dotnet publish TripNest.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out .
# Copy migration files explicitly
COPY --from=build /app/Migrations ./Migrations
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TripNest.dll"]