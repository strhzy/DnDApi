.PHONY update:
    docker compose down -v
    docker rmi dndapi:latest
    git pull
    docker compose up -d