# browsercontrol

A remote control webApp for the balenablocks/browser block. Simply point it at your instance of the browser block, and configure it remotely:

![Screenshot](https://i.ibb.co/yyptsXx/browsercontrol.png)

## Usage
Add this service to your docker-compose file:

```yaml
webapp:
    image: balenablocks/browsercontrol
    restart: always
    network_mode: host
    privileged: true
    ports:
      - 80:80
```
