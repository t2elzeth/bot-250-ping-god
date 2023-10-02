docker compose down

docker volume rm bot-250-ping-god-db-data

docker compose up -d

Sleep 2

.\migrate.cmd

.\migrate-seeds.cmd

