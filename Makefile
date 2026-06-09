.PHONY: help build up down restart hard-restart logs migrate seed test run-no-container watch

help:
	@echo ""
	@echo "Orbital Guardian API — Targets disponíveis:"
	@echo ""
	@echo "  build            Constrói a imagem Docker"
	@echo "  up               Sobe o container da API em background"
	@echo "  down             Para e remove os containers"
	@echo "  restart          Reinicia o container da API (sem rebuild)"
	@echo "  hard-restart     Para tudo, rebuilda a imagem e sobe novamente"
	@echo "  logs             Exibe os logs da API em tempo real"
	@echo "  migrate          Executa as migrations do EF Core dentro do container"
	@echo "  seed             Faz login como admin e importa dados TLE via API"
	@echo "  test             Executa todos os testes unitários e de integração"
	@echo "  run-no-container Executa a API localmente sem Docker (porta 5299)"
	@echo "  watch            Executa a API localmente com hot reload (porta 5299)"
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

hard-restart:
	@echo "Parando containers..."
	docker compose down
	@echo "Reconstruindo imagem..."
	docker compose build
	@echo "Subindo containers..."
	docker compose up -d
	@echo "API disponível em http://localhost:8080/swagger"

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

run-no-container:
	@echo ""
	@echo "Rodando a API localmente (sem Docker)..."
	@echo "  API:     http://localhost:5299"
	@echo "  Swagger: http://localhost:5299/swagger"
	@echo ""
	dotnet run --project src/OrbitalGuardian.API

watch:
	@echo ""
	@echo "Iniciando hot reload (sem Docker)..."
	@echo "  API:     http://localhost:5299"
	@echo "  Swagger: http://localhost:5299/swagger"
	@echo "  Salve um arquivo .cs para recarregar automaticamente."
	@echo ""
	dotnet watch run --project src/OrbitalGuardian.API
