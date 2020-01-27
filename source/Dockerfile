FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /source
COPY . .
RUN dotnet restore "Ui/MongoDocker.Sample.Ui.Api/MongoDocker.Sample.Ui.Api.csproj"
WORKDIR "/source/Ui/MongoDocker.Sample.Ui.Api"
RUN dotnet build "MongoDocker.Sample.Ui.Api.csproj" -c Release -o /app
RUN dotnet publish "MongoDocker.Sample.Ui.Api.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
RUN rm *.pdb *.Development.json
ENTRYPOINT ["dotnet", "MongoDocker.Sample.Ui.Api.dll"]