# Deploying AlSoque to the VPS

The VPS already has Docker, `shared-net`, the `postgres` container, and all
sibling projects (A3lam / AlMahmoud / Modawana / SmartPharm / StoreDz) running.
AlSoque joins the same `shared-net` and uses the same `postgres` host —
no separate database container.

## 1. Lay down the compose file

On the **VPS** as `root`:

```bash
mkdir -p /opt/AlSoque
# then put docker-compose.yml from the repo root into /opt/AlSoque/
# (scp, copy/paste, or any transfer)
```

## 2. Set a real admin password

The default in `docker-compose.yml` (`ChangeMe123!`) is a placeholder. Put the
real one in `/opt/AlSoque/.env` (compose auto-loads it):

```env
SeedAdmin__Email=admin@alsoque.net
SeedAdmin__Password=YourStrongPassword!
```

## 3. First pull + up

No manual database/migration step needed — `Program.cs` calls
`Database.MigrateAsync()` on startup, which creates `AlSoqueDb` on the shared
`postgres` container if it doesn't exist yet and applies all migrations.

```bash
cd /opt/AlSoque
docker compose pull
docker compose up -d --remove-orphans
docker compose ps
docker compose logs -f web
```

## 4. Nginx for `www.alsoque.net`

`/etc/nginx/sites-available/alsoque.net`:

```nginx
# Map for the WebSocket Upgrade header used by Blazor Server.
map $http_upgrade $connection_upgrade {
    default upgrade;
    ''      close;
}

server {
    listen 80;
    server_name www.alsoque.net alsoque.net;

    location /.well-known/acme-challenge/ { root /var/www/html; }
    location / { return 301 https://www.alsoque.net$request_uri; }
}

server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name www.alsoque.net alsoque.net;

    ssl_certificate     /etc/letsencrypt/live/www.alsoque.net/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/www.alsoque.net/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers off;

    # Blazor Server: long-lived WebSocket + large antiforgery headers.
    location / {
        proxy_pass http://127.0.0.1:8089;
        proxy_http_version 1.1;
        proxy_set_header Host              $host;
        proxy_set_header X-Real-IP         $remote_addr;
        proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Upgrade           $http_upgrade;
        proxy_set_header Connection        $connection_upgrade;
        proxy_read_timeout 3600s;
        proxy_send_timeout 3600s;
        client_max_body_size 25m;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/alsoque.net /etc/nginx/sites-enabled/
sudo nginx -t
sudo certbot --nginx -d www.alsoque.net -d alsoque.net
sudo systemctl reload nginx
```

## 5. Azure DevOps (`DeployToVPS.yml`)

Required service connections (already exist in the Azure DevOps project from
the sibling apps — reuse them, no new connection needed):

| Name         | Type            | Purpose                          |
| ------------ | --------------- | --------------------------------- |
| `ghcr-login` | Docker Registry | `docker login ghcr.io`           |
| `vps-ssh`    | SSH             | deploy job to the VPS            |

Trigger: push to `master` → `BuildAndPush` (builds + pushes to
`ghcr.io/nadir-mezhoudi/alsoque-web`) → `Deploy` (SSH, `docker compose pull && up -d`).

Rollback: re-run the pipeline with an older `Build.BuildNumber`, or SSH in
and `docker compose pull ghcr.io/nadir-mezhoudi/alsoque-web:<sha>`.

## Port allocation on this VPS (avoid collisions)

| Project    | Host port (127.0.0.1:`<port>`) |
| ---------- | ------------------------------- |
| A3lam      | 8080                            |
| AlMahmoud  | 8088                            |
| AlSoque    | 8089                            |

Confirm `8089` is still free on the VPS before first deploy (`docker ps`,
`ss -ltnp | grep 8089`) — pick another port in `docker-compose.yml` and this
Nginx config if it's already taken by another app not listed above.
