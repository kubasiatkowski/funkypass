# Funky password generator

I'm mostly the infra guy who was annoyed with random passwords and wanted to provide something more human oriented to end users which triggered me to code multiple implementations of dictionary based passphrase generators and made one of them opensource. The project was created for fun to play a little bit with serverless architecture and it is built on Azure Functions, Azure SQL with Bootstrap and Vue.js Front-end.

To see it in inaction go to https://funkypass.interkreacja.pl/ , you can also use API to integrate our password generator with your projects. 
(please be aware that it is hosted as cheap as possible and takes a while to heat up workers when they are suspended)

## Api 
Do you want to integrate our passwords with your user provisioning process? Or maybe you hate GUI and don’t want to leave a console to generate a nice password? It’s super easy, just call our REST API. The description is available on [SwaggerHub](https://app.swaggerhub.com/apis/interkreacja/funkypass-interkreacja_pl/1.0.0).

### Api usage examples 
Generate password in PowerShell

``` powershell
$response = Invoke-RestMethod -Uri "https://funkypass.interkreacja.pl/api/GeneratePassword?lang=la"
$funkypassword = $response.password
Write-Host $funkypassword
 ```
 
Generate password in bash

``` sh
curl https://funkypass.interkreacja.pl/api/GeneratePassword?lang=la -silent | sed -n -e 's/^.*password":"//p' | cut -d'"' -f1
```                        
