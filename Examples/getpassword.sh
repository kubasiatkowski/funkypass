#!/bin/bash
curl https://funkypass.interkreacja.pl/api/GeneratePassword?lang=la -silent | sed -n -e 's/^.*password":"//p' | cut -d'"' -f1