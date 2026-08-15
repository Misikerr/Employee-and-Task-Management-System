FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EmployeeTaskManagement.csproj", "./"]
RUN dotnet restore "EmployeeTaskManagement.csproj"
COPY . .
RUN dotnet publish "EmployeeTaskManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EmployeeTaskManagement.dll"]
