#!/bin/sh

docker build -t tlaguz/docker-examples:request-integrity-backend .
docker push tlaguz/docker-examples:request-integrity-backend
