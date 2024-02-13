.PHONY: run-docker
run-docker:
    @echo "Running docker-compose to spin up all the components..."
	@docker-compose down --remove-orphans
    @docker-compose up --build --force-recreate -d
    @echo "Done"

.PHONY: stop-docker
stop-docker:
    @echo "Stopping all components running on Docker..."
    @docker-compose down --remove-orphans
    @echo "Done"
