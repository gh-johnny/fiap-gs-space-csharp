# Orbital Guardian API

## Descrição

**Orbital Guardian** é uma API REST para monitoramento de conjunções orbitais — situações em que dois objetos espaciais (satélites, detritos ou estações) se aproximam a uma distância perigosa. O sistema avalia a probabilidade de colisão, emite alertas automáticos e gerencia o ciclo de vida das conjunções detectadas com autenticação JWT e controle de acesso baseado em papéis.

O projeto implementa os princípios de **Domain-Driven Design (DDD)** com arquitetura em camadas (Domain → Application → Infrastructure → API), CQRS manual com dispatchers via reflexão, State Machine para transições de estado, padrão Builder para criação de aggregates complexos, e resiliência via Polly (retry + timeout). Os dados orbitais são importados no formato TLE (Two-Line Element, padrão NORAD) e a persistência é feita com SQLite via Entity Framework Core.

## Conexão com os ODS

| ODS | Conexão |
|-----|---------|
| **ODS 8** — Trabalho Decente e Crescimento Econômico | Automação do monitoramento orbital reduz o custo operacional de missões espaciais |
| **ODS 9** — Indústria, Inovação e Infraestrutura | Infraestrutura crítica para proteção de satélites de comunicação e observação |
| **ODS 11** — Cidades e Comunidades Sustentáveis | Satélites de observação dependem de órbitas seguras para monitorar desastres naturais |
| **ODS 13** — Ação Contra a Mudança Global do Clima | Satélites climáticos e meteorológicos requerem proteção contra colisões orbitais |

## Integrantes

| Nome | RM |
|------|----|
| João Monteiro de Furtado Romero | RM559154 |

## Tecnologias

- **.NET 8** / **ASP.NET Core 8** — framework web
- **Entity Framework Core 8** + **SQLite** — ORM e banco de dados
- **Polly** — resiliência: retry exponencial + timeout por repositório
- **JWT Bearer** — autenticação stateless com HMAC-SHA256
- **BCrypt.Net** — hash de senhas com work factor 12
- **Docker** + **Docker Compose** — containerização e orquestração
- **xUnit** + **Moq** + **FluentAssertions** — testes unitários e de integração
- **Swashbuckle** — Swagger UI com suporte a JWT

## Execução

### Via Docker (recomendado)

```bash
# Clonar o repositório
git clone https://github.com/<usuario>/orbital-guardian-api.git
cd orbital-guardian-api

# Build e start
make build
make up

# API disponível em:
# http://localhost:8080/swagger
```

### Local (sem Docker)

> Requer `aspnet-runtime-8.0` instalado.

```bash
dotnet run --project src/OrbitalGuardian.API
```

### Credenciais do admin padrão

| Campo | Valor |
|-------|-------|
| Email | `admin@orbitalguardian.com` |
| Senha | `Admin@123` |

### Seed de dados

Após o `make up`, o sistema já cria o admin automaticamente. Para importar dados TLE:

```bash
make seed
```

## Makefile — targets disponíveis

| Target | Descrição |
|--------|-----------|
| `make build` | Build da imagem Docker |
| `make up` | Sobe a API em background |
| `make down` | Para e remove os containers |
| `make logs` | Streama logs da API |
| `make seed` | Login + importação de TLEs |
| `make test` | Executa a suite de testes |
| `make run-local` | Executa a API localmente |

## Diagrama de Classes

```mermaid
classDiagram
    class AggregateRoot~TId~ {
        +TId Id
        +RaiseDomainEvent()
        +GetDomainEvents()
        +ClearDomainEvents()
    }

    class Entity~TId~ {
        +TId Id
        +Equals()
    }

    class ValueObject {
        <<abstract>>
        #GetEqualityComponents()*
    }

    class IBuildable {
        <<interface>>
    }

    class IStateMachine~TState~ {
        <<interface>>
        +CurrentState TState
        +CanTransitionTo(state) bool
        +TransitionTo(state)
    }

    class IBuilder~T~ {
        <<interface>>
        +Build() T
    }

    class SpaceObject {
        <<abstract>>
        +Name string
        +NoradId string
        +Type SpaceObjectType
        +OrbitalElements OrbitalElements
        +IsActive bool
        +RecordTelemetry()
        +Deactivate()
    }

    class Satellite {
        +Operator string
        +MissionType string
        +MassKg double
        +Create()$
    }

    class SpaceDebris {
        +OriginObject string
        +EstimatedSizeM double
        +Create()$
    }

    class SpaceStation {
        +CrewCapacity int
        +Agency string
        +Create()$
    }

    class ConjunctionEvent {
        +PrimaryObjectId Guid
        +SecondaryObjectId Guid
        +Status ConjunctionStatus
        +CollisionProbability
        +MissDistance
        +Alerts AlertCollection
        +Create()$
        +Resolve()
        +AcknowledgeAlert()
    }

    class Alert {
        +Severity AlertSeverity
        +Status AlertStatus
        +Acknowledge()
    }

    class User {
        +Email string
        +Role UserRole
        +Create()$
        +UpdateProfile()
        +Deactivate()
    }

    class ConjunctionEventBuilder {
        +WithPrimaryObject()
        +WithSecondaryObject()
        +WithPredictedTca()
        +WithMissDistance()
        +WithCollisionProbability()
        +Build()
    }

    class TleParser {
        <<static>>
        +Parse(line1, line2)$
    }

    class OrbitalElements {
        +Inclination double
        +Eccentricity double
        +MeanMotion double
        +Create()$
    }

    AggregateRoot~TId~ --|> Entity~TId~
    SpaceObject --|> AggregateRoot~Guid~
    Satellite --|> SpaceObject
    SpaceDebris --|> SpaceObject
    SpaceStation --|> SpaceObject
    ConjunctionEvent --|> AggregateRoot~Guid~
    ConjunctionEvent ..|> IStateMachine~ConjunctionStatus~
    ConjunctionEvent ..|> IBuildable
    Alert --|> Entity~Guid~
    Alert ..|> IStateMachine~AlertStatus~
    User --|> AggregateRoot~Guid~
    ConjunctionEventBuilder ..|> IBuilder~ConjunctionEvent~
    OrbitalElements --|> ValueObject
    ConjunctionEvent "1" *-- "many" Alert
```

## Diagrama de Arquitetura

```mermaid
flowchart TB
    subgraph Client
        HTTP[HTTP Client / Swagger UI]
    end

    subgraph API["API Layer"]
        GEM[GlobalExceptionHandlingMiddleware]
        AUTH[JWT Authentication]
        CTRL[Controllers\nAuth · Users · SpaceObjects\nConjunctions · Alerts]
    end

    subgraph Application["Application Layer (CQRS)"]
        CMD[CommandDispatcher]
        QRY[QueryDispatcher]
        EVT[DomainEventDispatcher]
        HANDLERS[Command/Query Handlers]
        EH[Domain Event Handlers]
    end

    subgraph Domain["Domain Layer"]
        AGG[Aggregates\nSpaceObject · ConjunctionEvent · User]
        SM[State Machine\nIStateMachine]
        VO[Value Objects\nOrbitalElements · CollisionProbability · MissDistance]
        TLE[TleParser]
        SVCDOM[IRiskAssessmentService]
    end

    subgraph Infrastructure["Infrastructure Layer"]
        REPOS[Repositories\n+ Polly Retry/Timeout]
        DB[(SQLite\nEF Core)]
        GW[MockTleDataGateway\nSpaceTrackTleGateway]
        JWT_SVC[JwtTokenService]
        BCRYPT[BCryptPasswordHasher]
        SEEDER[DatabaseSeeder]
    end

    HTTP --> GEM --> AUTH --> CTRL
    CTRL --> CMD & QRY
    CMD & QRY --> HANDLERS
    HANDLERS --> EVT --> EH
    HANDLERS --> AGG
    AGG --> SM & VO
    TLE --> VO
    SVCDOM --> AGG
    HANDLERS --> REPOS & GW & JWT_SVC & BCRYPT
    REPOS --> DB
    SEEDER --> DB
```
