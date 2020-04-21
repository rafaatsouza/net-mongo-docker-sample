FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "Ui/MongoDockerSample.Ui.Api/MongoDockerSample.Ui.Api.csproj"
WORKDIR "/source/Ui/MongoDockerSample.Ui.Api"
RUN dotnet build "MongoDockerSample.Ui.Api.csproj" -c Release -o /app
RUN dotnet publish "MongoDockerSample.Ui.Api.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
RUN rm *.pdb *.Development.json
ENTRYPOINT ["dotnet", "MongoDockerSample.Ui.Api.dll"]