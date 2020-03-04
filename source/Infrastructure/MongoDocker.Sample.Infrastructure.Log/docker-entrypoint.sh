#!/bin/sh -e

echo 'starting'

SERVICE="http://logseq"

until curl -fs ${SERVICE}  > /dev/null; 
do
  >&2 echo "${SERVICE} is unavailable - sleeping"
  sleep 2
done
>&2 echo "${SERVICE} is up"

/bin/seqcli/seqcli apikey create --title='mongo-docker-api' --token='bEK8BeLxVlWi3cvgdBSe0D7ub8nix1p5' --server=$SERVICE

exit 0
