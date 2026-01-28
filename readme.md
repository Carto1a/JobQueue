# JobQueue Service

Background job processing system built with **.NET**, **MongoDB**, and **RabbitMQ**, following **Clean Architecture** principles.

The API creates jobs, workers process them asynchronously, and a dispatcher guarantees delivery even if RabbitMQ is temporarily unavailable.

# Architecture

The solution is split by responsibility, not by technology.

```
src/
├── JobQueue.Api            # HTTP entrypoint (controllers)
├── JobQueue.Application    # Use cases / business orchestration
├── JobQueue.Domain         # Core business rules
├── JobQueue.Infrastructure # MongoDB, RabbitMQ, external implementations
├── JobQueue.Worker         # Processes jobs from queue
└── JobQueue.Dispatcher     # Background service that republishes pending jobs
```

## Worker

Consumes jobs from RabbitMQ and executes:

```
Queue → Worker → ProcessJobHandler → Domain → DB
```

## Dispatcher

Background service that ensures **reliability**.

If RabbitMQ is down when a job is created, the job stays in DB with `Created` status.

Dispatcher periodically:

```
DB (Created) → Publish → Mark as Pending
```

# Job Lifecycle

```
Created → Pending (Queued) → Processing → Completed
                               ↓
                              Failed → Retry
```

# Running the Project

### Requirements

- Docker
- Docker Compose

## Start everything

```bash
docker compose up --build --scale worker=2
```

Services:

| Service | Port |
|--------|------|
| API | 8081 |
| RabbitMQ UI | 15672 |
| MongoDB | 27017 |

## RabbitMQ Management

http://localhost:15672

## ENV

```env
MONGO_DATABASE=jobs
MONGO_USERNAME=jobsuser
MONGO_PASSWORD=jobspasswordgood123
RABBITMQ_USERNAME=coelho
RABBITMQ_PASSWORD=coelhosenha67
```

# API Endpoints

## Create Job

```http
POST http://localhost:8081/jobs
Content-Type: application/json

{
  "jobType": 0,
  "payload": "teste2"
}
```

## Get All Jobs

```http
GET http://localhost:8081/jobs
```

## Get Job by Id

```http
GET http://localhost:8081/jobs/3aa43d77-c2fb-4348-b0d0-c2b324db2cae
```

# Stack

- .NET
- MongoDB
- RabbitMQ
- Docker

