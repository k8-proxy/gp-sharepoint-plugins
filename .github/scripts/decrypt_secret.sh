#!/bin/sh

mkdir $HOME/secrets

gpg --quiet --batch --yes --decrypt --passphrase="$SPOAUTH_SECRET_PASSPHRASE" --output $HOME/secrets/spo_auth.json /home/runner/work/gp-sharepoint-plugins/gp-sharepoint-plugins/Online/o365-filehandler-tests/filehandler-tests/env/default/spo_auth.json.gpg