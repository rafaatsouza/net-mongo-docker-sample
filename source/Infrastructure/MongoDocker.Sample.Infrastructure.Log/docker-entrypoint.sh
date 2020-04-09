#!/bin/sh -e

TOKEN=$1

echo 'starting'
echo "> ${TOKEN}"

SERVICE="http://logseq"

until curl -fs ${SERVICE}  > /dev/null; 
do
  >&2 echo "${SERVICE} is unavailable - sleeping"
  sleep 1
done
>&2 echo "${SERVICE} is up"

/bin/seqcli/seqcli apikey create --title='mongo-docker-api' --token='${1}' --server=${SERVICE}

exit 0
