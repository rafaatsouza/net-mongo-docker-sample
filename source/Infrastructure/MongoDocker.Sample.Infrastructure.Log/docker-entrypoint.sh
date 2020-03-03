#!/bin/sh -e

echo 'starting'

SERVICE="http://logseq"

until curl -fs $SERVICE  > /dev/null; 
do
  >&2 echo "$SERVICE is unavailable - sleeping"
  sleep 2
done
>&2 echo "$SERVICE is up"

seqcli apikey create --title='newapikey' --token='123456' --server=$SERVICE

exit 0
