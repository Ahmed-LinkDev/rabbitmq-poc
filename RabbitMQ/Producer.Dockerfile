FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app/RabbitMQ-Producer

# copy csproj and restore as distinct layers
COPY RabbitMQ-Producer/*.csproj ./RabbitMQ-Producer/
COPY RabbitMQ-Commons/*.csproj ./RabbitMQ-Commons/


RUN dotnet restore "RabbitMQ-Producer/RabbitMQ-Producer.csproj"
# copy everything else and build app
COPY RabbitMQ-Producer/. ./RabbitMQ-Producer/
COPY RabbitMQ-Commons/. ./RabbitMQ-Commons/
COPY .env  .

RUN dotnet publish -c Release -o out "RabbitMQ-Producer/RabbitMQ-Producer.csproj"

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app/Resourcer
COPY --from=build /app/RabbitMQ-Producer/out ./

ENTRYPOINT ["./RabbitMQ-Producer"]
