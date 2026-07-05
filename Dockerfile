# Use the official Microsoft .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies
COPY SRT_group_todo_list/*.csproj ./SRT_group_todo_list/
RUN dotnet restore ./SRT_group_todo_list/SRT_group_todo_list.csproj

# Copy the remaining source files and build the release
COPY SRT_group_todo_list/ ./SRT_group_todo_list/
WORKDIR /app/SRT_group_todo_list
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Create directory for persistent SQLite database and mark as volume
RUN mkdir -p /app/data
VOLUME /app/data

# Set connection string via Environment Variable to use the persistent data directory
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/todo.db"
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80

ENTRYPOINT ["dotnet", "SRT_group_todo_list.dll"]
