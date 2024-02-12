FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "Ui/MongoDockerSample.Ui.Api/MongoDockerSample.Ui.Api.csproj"
WORKDIR "/source/Ui/MongoDockerSample.Ui.Api"
RUN dotnet build "MongoDockerSample.Ui.Api.csproj" -c Release -o /app
RUN dotnet publish "MongoDockerSample.Ui.Api.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
RUN rm *.pdb *.Development.json
ENTRYPOINT ["dotnet", "MongoDockerSample.Ui.Api.dll"]
