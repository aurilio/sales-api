#!/bin/bash
# wait-for-postgres.sh

host="$1"
shift
cmd="$@"

until pg_isready -h "$host" -p 5432 > /dev/null 2>&1; do
  >&2 echo "Postgres não está pronto ainda... esperando"
  sleep 1
done

>&2 echo "Postgres está pronto! Iniciando aplicação..."
exec $cmd