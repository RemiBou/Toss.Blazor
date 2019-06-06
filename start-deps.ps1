docker-machine start default; 
docker-machine env default | Invoke-Expression ; 
docker-compose up -d ravendb
