# SoftwareTesting.Performance

گزارش کار در ادرس زیر موجود است:
```
./docs/performance-testing-document.pdf
```

Limited required servers:
```

docker run -d --name rabbitmq-limited --memory=200m  --cpus=0.2 -p 5672:5672 -p 15672:15672 rabbitmq:4.0.4-management-alpine

docker run -d  --name sqlserver-limited  --memory=1.75GB   --cpus=0.5  -e "ACCEPT_EULA=Y"  -e "SA_PASSWORD=1234@Abcd"  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-CU16-ubuntu-22.04

docker run -d  --name mongodb-limited  --memory=200m  --cpus=0.2 -p 27017:27017  -e MONGO_INITDB_ROOT_USERNAME=admin  -e MONGO_INITDB_ROOT_PASSWORD=secret  mongo:latest

```