#!/bin/bash
docker-compose down -v
docker-compose up --renew-anon-volumes --exit-code-from cypress --build