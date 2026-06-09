.PHONY: help build up down restart logs migrate seed test run-local

help:
	@echo ""
	@echo "Orbital Guardian API — Targets disponíveis:"
	@echo ""
	@echo "  build       Constrói a imagem Docker"
	@echo "  up          Sobe o container da API em background"
	@echo "  down        Para e remove os containers"
	@echo "  restart     Reinicia o container da API"
	@echo "  logs        Exibe os logs da API em tempo real"
	@echo "  migrate     Executa as migrations do EF Core dentro do container"
	@echo "  seed        Faz login como admin e importa dados TLE via API"
	@echo "  test        Executa todos os testes unitários e de integração"
	@echo "  run-local   Executa a API localmente sem Docker"
	@echo ""

build:
	docker compose build

up:
	docker compose up -d
	@echo "API disponível em http://localhost:8080/swagger"

down:
	docker compose down

restart:
	docker compose restart api

logs:
	docker compose logs -f api

migrate:
	docker compose exec api dotnet ef database update

seed:
	@echo "Fazendo login como admin..."
	$(eval TOKEN := $(shell curl -s -X POST http://localhost:8080/api/auth/login \
		-H "Content-Type: application/json" \
		-d '{"email":"admin@orbitalguardian.com","password":"Admin@123"}' \
		| python3 -c "import sys,json; print(json.load(sys.stdin)['token'])"))
	@echo "Importando dados TLE..."
	curl -s -X POST http://localhost:8080/api/space-objects/import-tle \
		-H "Authorization: Bearer $(TOKEN)" \
		-H "Content-Type: application/json" | python3 -m json.tool
	@echo "Concluído."

test:
	dotnet test tests/OrbitalGuardian.Tests --logger "console;verbosity=normal"

run-local:
	dotnet run --project src/OrbitalGuardian.API
