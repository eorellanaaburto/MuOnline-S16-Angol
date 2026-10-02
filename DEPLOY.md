# Despliegue MU Online Season 16 Kor (MuEmu)

Fork de [Yomalex/MuEmu](https://github.com/Yomalex/MuEmu) con los archivos de despliegue Docker
en `docker/` y los fixes necesarios para correr en Linux (el proyecto original está pensado para
Windows). El código fuente del servidor (`MuEmu/`, `CSEmu/`, etc.) es tal cual lo publica el autor
original — no se tocó para poder traer sus actualizaciones con `git pull upstream master`.

## Estado actual

Corriendo en `129.1.5.170` (TrueNAS SCALE, Docker, `network_mode: host`):

| Servicio | Puerto | Rol |
|---|---|---|
| ConnectServer | 44405 | Lista de servidores |
| ConnectServer (chat) | 55980 | Chat |
| GameServer | 55901 | Juego |
| MySQL | 3306 (solo localhost) | Base de datos |

Cliente compatible: Season 16 Korean, `Version=11946`, `Serial=fughy683dfu7teqg`.

## Bugs del proyecto original corregidos para Linux

Todos están parcheados en `docker/Dockerfile.gameserver` (no en el código fuente), así que
sobreviven a actualizaciones futuras vía `git pull upstream master`:

1. **Separadores de ruta `\` estilo Windows** en `Data/ItemBags/ItemBags.xml` (ej.
   `Bag="Moon Rabbit\BlueEvent.xml"`) — rompen en Linux, que es case/separator-sensitive.
   Normalizado a `/` en build time.
2. **Case-sensitivity**: el código pide `MasterSkillTree_Season16Kor.xml` (K mayúscula) pero el
   archivo se llama `MasterSkillTree_Season16kor.xml` (k minúscula). Se copia con el nombre correcto
   en build time.
3. **Bug real de la app** (no solo de portabilidad): en `MuEmu/Program.cs`, varios campos de
   `<Files>` en `Server.xml` (`Monsters`, `MapServer`, `MonsterSetBase`, `SelupanPatterns`,
   `QuestWorld`) se concatenan con `DataRoot` en el código, pero sus valores por defecto YA incluyen
   el prefijo `./Data/`, duplicándolo (`./Data/./Data/...`) y rompiendo la carga de monstruos/mapas
   silenciosamente (sin excepción, solo "0 clases cargadas"). Se corrige en `Server.xml`, quitando
   el prefijo de esos 5 campos únicamente (los demás campos `Files.*` SÍ llevan el prefijo completo
   correctamente).

## Desplegar / actualizar en el servidor

```bash
cd ~/muonline   # o donde esté clonado este repo en el servidor
git pull origin master          # trae nuestros fixes/cambios
# o, para traer novedades del autor original primero:
# git fetch upstream && git merge upstream/master && git push origin master

cd docker
cp .env.example .env            # solo la primera vez, luego editar con password real
docker compose build            # o: docker compose pull (si ya corrió el CI de GitHub Actions)
docker compose up -d
```

Primera vez únicamente — crear el esquema de base de datos:
```bash
docker compose up -d mysql
# esperar a que mysql esté listo, luego:
docker compose up -d connectserver gameserver
printf 'db create\n' | docker attach mu-gameserver   # Ctrl+C si no vuelve el prompt
docker restart mu-gameserver
```

Las plantillas de configuración están en `docker/config-templates/` — copiarlas a
`docker/config/gameserver/Server.xml` y `docker/config/connectserver/configuration.xml`
(ese directorio está en `.gitignore`, no se versiona) y reemplazar `CHANGE_ME_MYSQL_ROOT_PASSWORD`.

## CI (GitHub Actions)

En cada push a `master` que toque el código del servidor o `docker/`, se compilan y publican las
imágenes a GHCR:
- `ghcr.io/eorellanaaburto/muonline-s16-gameserver:latest`
- `ghcr.io/eorellanaaburto/muonline-s16-connectserver:latest`

Nota: la primera vez hay que marcar esos paquetes como públicos en GitHub (Settings del repo →
Packages), o configurar `docker login ghcr.io` en el servidor con un token, para poder hacer
`docker compose pull`.

## Cliente

El cliente del juego (assets, ~2GB) **no está en este repo** — son archivos del juego con copyright
de Webzen, no corresponde subirlos a GitHub. Se gestionan aparte (OneDrive/local).

Config del cliente (`Config.ini`) debe apuntar a la IP del servidor:
```ini
[MU]
Version=11946
URL = 129.1.5.170
Serial = fughy683dfu7teqg
Port = 44405
```

## Migración de datos desde otro servidor (pendiente)

Si en el futuro se migra data de personajes/inventario desde otro server (ej. OpenMU), no hay
herramienta automática — requiere un script a medida, y hay riesgo real de incompatibilidad de
ítems entre seasons distintas. Ver conversación original para detalles.
