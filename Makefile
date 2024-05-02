.PHONY: default update clean run

default: run

update: clean run

clean:
	docker rmi --force headers.security.api

run:
	docker-compose up
