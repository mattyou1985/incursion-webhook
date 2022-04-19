# Incursion Webhook
A simple service to post ESI incursion updates to Discord via a webhook.

**Bug Reports, Change Requests**  
Create an [Issue](/../../issues) to report a bug, request a feature OR you can contribute. 

When contributing do not push code directly to MAIN. Code should be created on DEV or a separate branch and merged into DEV for testing. When code is pushed to DEV a GitHub action will create a Docker image tagged `:dev`. Deploy this to a testing environment and if everything works you can create a PR against main.


**Deployment**  
You can host this service on any machine that has support for Docker & Docker Compose. 

1. Create a file `docker-compose.yml`, Use the compose file in this repo as a template
2. Chose a version of the docker image for this repo that suits your needs
   * `:latest` - updated from the 'main' branch each time code is changed (maybe unstable)
   * `:dev:` - code from the 'dev' branch, use this for testing and consider it to be unstable
   * Specific stable and tested versions may also be available


3. Start the service `$ docker-compose up`, and navigate to `http://<server_ip>:8093/swagger/index.html` where you should register your webhooks
4. Change the `ASPNETCORE_ENVIRONMENT` to `Production` and run `$ docker-compose down && docker-compose up` - This will disable Swagger UI, you may also choose to remove the `PORTS` for the api service

