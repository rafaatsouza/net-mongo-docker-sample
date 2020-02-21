FROM datalust/seqcli:latest
WORKDIR /source
COPY . .
RUN ["chmod", "+x", "setup.sh"]
# ENTRYPOINT ["/bin/sh", "-c", "ls -l"] 
ENTRYPOINT ["/bin/sh", "setup.sh"]