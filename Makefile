.PHONY: help build up down restart logs migrate seed test run-local

help:
	@echo ""
	@echo "Orbital Guardian API — Available targets:"
	@echo ""
	@echo "  build       Build Docker image"
	@echo "  up          Start API container in background"
	@echo "  down        Stop and remove containers"
	@echo "  restart     Restart API container"
	@echo "  logs        Stream API container logs"
	@echo "  migrate     Run EF Core database migrations inside container"
	@echo "  seed        Login as admin and import TLE data via API"
	@echo "  test        Run all unit/integration tests"
	@echo "  run-local   Run API locally without Docker"
	@echo ""

build:
	docker compose build

up:
	docker compose up -d
	@echo "API running at http://localhost:8080/swagger"

down:
	docker compose down

restart:
	docker compose restart api

logs:
	docker compose logs -f api

migrate:
	docker compose exec api dotnet ef database update

seed:
	@echo "Logging in as admin..."
	$(eval TOKEN := $(shell curl -s -X POST http://localhost:8080/api/auth/login \
		-H "Content-Type: application/json" \
		-d '{"email":"admin@orbitalguardian.com","password":"Admin@123"}' \
		| python3 -c "import sys,json; print(json.load(sys.stdin)['token'])"))
	@echo "Importing TLE data..."
	curl -s -X POST http://localhost:8080/api/space-objects/import-tle \
		-H "Authorization: Bearer $(TOKEN)" \
		-H "Content-Type: application/json" | python3 -m json.tool
	@echo "Done."

test:
	dotnet test tests/OrbitalGuardian.Tests --logger "console;verbosity=normal"

run-local:
	dotnet run --project src/OrbitalGuardian.API
