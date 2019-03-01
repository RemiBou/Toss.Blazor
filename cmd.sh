brew cask install virtuallbox
docker-machine create myvm1 -d virtualbox
docker-machine create myvm2 -d virtualbox
docker-machine create manager1 -d virtualbox
docker secret create tossserver  .microsoft/usersecrets/ae68635e-649c-4f23-a39b-24a5ea3987f4/secrets.json #create the secret file as an available secret
#192.168.99.102
docker-machine ssh manager1
docker swarm init # creates the swarm
docker swarm join-token worker # display command for joining the swarm on the worker
eval $(docker-machine env manager1) # all the following docker cmd will be evaluated by manager1 with file on current computer
docker service create --name registry --publish published=5000,target=5000 registry:2 #creates a local registry
docker-compose build # build the images in the compose, swamr does not support it
docker-compose push # publish the images to the local registry
docker stack deploy -c docker-compose.yml tossstack