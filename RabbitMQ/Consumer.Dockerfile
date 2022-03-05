FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app/RabbitMQ-Consumer

# copy csproj and restore as distinct layers
COPY RabbitMQ-Consumer/*.csproj ./RabbitMQ-Consumer/
COPY RabbitMQ-Commons/*.csproj ./RabbitMQ-Commons/


RUN dotnet restore "RabbitMQ-Consumer/RabbitMQ-Consumer.csproj"
# copy everything else and build app
COPY RabbitMQ-Consumer/. ./RabbitMQ-Consumer/
COPY RabbitMQ-Commons/. ./RabbitMQ-Commons/
COPY .env  .

RUN dotnet publish -c Release -o out "RabbitMQ-Consumer/RabbitMQ-Consumer.csproj"

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app/RabbitMQ-Consumer
COPY --from=build /app/RabbitMQ-Consumer/out ./

ENTRYPOINT ["./RabbitMQ-Consumer"]