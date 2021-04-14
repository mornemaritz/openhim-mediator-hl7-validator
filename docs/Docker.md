# Docker Commands

##### Build docker image (replace repo name 'mornemaritz' with yours)
`docker build -t mornemaritz/hl7-validator:0.0.10 ---label "notes=no-registration.no-heartbeat" .`

##### Run docker container locally, connected to openhim container network
`docker-compose -f docker-compose.dev.yml up -d`

`docker-compose -f docker-compose.dev.yml down`

