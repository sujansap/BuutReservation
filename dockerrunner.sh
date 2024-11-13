#!/bin/bash
# Exit immediately if a command exits with a non-zero status
set -e
# Treat unset variables as an error when substituting
set -u
# Print each command before executing it (for debugging purposes)
set -x

export ConnectionStrings__PostgreSQL="user ID=${DB_USERNAME};Password=${DB_PASSWORD};Host=${DB_IP};Port=${DB_PORT};Database=${DB_NAME};SSL Mode=Require;Trust Server Certificate=True;Connection Lifetime=0;"

./migrations

exec dotnet /app/Rise.Server.dll --urls "http://0.0.0.0:${HTTP_PORT};https://0.0.0.0:${HTTPS_PORT}" --environment ${ENVIREMENT} # Production
#docker run -it -e DB_USERNAME=Hogent -e DB_PASSWORD=Hogent2425 -e DB_IP=172.17.0.3 -e DB_PORT=5432 -e DB_NAME="Hogent.Rise" -e http_port=5000 -e https_port=5001  -p 5000:5000 -p 5001:5001 mydotnetapp /bin/bash


  
