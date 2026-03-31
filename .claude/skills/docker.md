Manage the Docker Compose environment for the Document Intelligence platform.

Project root: `/Users/goktugibolar/dev/document-intelligence`

Services: web-api, worker, postgres (pgvector), rabbitmq, seq

Commands:
- Start: `docker compose up -d`
- Stop: `docker compose down`
- Status: `docker compose ps`
- Logs: `docker compose logs -f {service}`
- Reset DB: `docker compose down -v` (WARNING: deletes all data)

If no action is specified in the args, ask: up / down / status / logs?
After starting, remind the user that:
- API is at http://localhost:5000
- RabbitMQ Management is at http://localhost:15672 (guest/guest)
- Seq logs are at http://localhost:8081
