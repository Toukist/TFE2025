# ?? Guide de d�ploiement - Dior Enterprise System

## ?? Checklist de mise en production

### ? Pr�requis serveur
- [ ] Windows Server 2019+ ou Linux Ubuntu 20.04+
- [ ] .NET 8 Runtime install�
- [ ] SQL Server 2019+ ou SQL Server Express
- [ ] IIS (Windows) ou Nginx/Apache (Linux)
- [ ] Certificat SSL valide

### ? Configuration base de donn�es

#### 1. Cr�ation de la base de donn�es
```sql
-- Ex�cuter en tant qu'administrateur SQL Server
CREATE DATABASE [Dior.Database.Production];
GO

-- Cr�er un utilisateur d�di� pour l'application
USE [Dior.Database.Production];
GO

CREATE LOGIN [DiorAppUser] WITH PASSWORD = 'MotDePasseComplexe123!';
CREATE USER [DiorAppUser] FOR LOGIN [DiorAppUser];

-- Donner les permissions n�cessaires
ALTER ROLE db_datareader ADD MEMBER [DiorAppUser];
ALTER ROLE db_datawriter ADD MEMBER [DiorAppUser];
ALTER ROLE db_ddladmin ADD MEMBER [DiorAppUser];
GO
```

#### 2. Ex�cution du script de cr�ation des tables
```bash
# Ex�cuter le script CreateMissingTables.sql sur la base de production
sqlcmd -S YourServer -d Dior.Database.Production -i CreateMissingTables.sql
```

### ? Configuration de l'application

#### 1. appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "Dior_DB": "Server=YourProductionServer;Database=Dior.Database.Production;User Id=DiorAppUser;Password=MotDePasseComplexe123!;TrustServerCertificate=false;Encrypt=true;"
  },
  "Jwt": {
    "Secret": "VotreCleSecreteProductionSecure256BitsMinimum2025Enterprise!",
    "Issuer": "DiorEnterpriseAPI.Production",
    "Audience": "DiorEnterpriseClient.Production",
    "ExpirationMinutes": 480
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com,*.yourdomain.com",
  "Security": {
    "RequireHttps": true,
    "AllowedOrigins": ["https://yourfrontend.com"]
  }
}
```

#### 2. Variables d'environnement (recommand�)
```bash
# D�finir les variables sensibles
export JWT__SECRET="VotreCleSecreteProductionSecure256BitsMinimum2025Enterprise!"
export CONNECTIONSTRINGS__DIOR_DB="Server=...;Database=...;User Id=...;Password=...;"
export EMAIL__SMTPPASSWORD="YourEmailPassword"
```

### ? Publication de l'application

#### Option A: Publication avec Visual Studio
1. Clic droit sur `Dior.Service.Host` ? Publish
2. Choisir "Folder" ou "IIS" selon votre serveur
3. Configuration: Release
4. Target Framework: net8.0
5. Deployment Mode: Framework-dependent
6. Publier

#### Option B: Publication en ligne de commande
```bash
# Se positionner dans le dossier du projet
cd Dior.Service.Host

# Publier l'application
dotnet publish -c Release -o ./publish --framework net8.0

# Cr�er un package zip pour le d�ploiement
tar -czf dior-enterprise-v1.0.tar.gz -C publish .
```

### ? Configuration IIS (Windows)

#### 1. Installation des modules requis
```powershell
# Installer le module ASP.NET Core
# T�l�charger depuis: https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer

# V�rifier l'installation
dotnet --info
```

#### 2. Configuration du site IIS
```xml
<!-- web.config g�n�r� automatiquement -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\Dior.Service.Host.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
    </system.webServer>
  </location>
</configuration>
```

### ? Configuration Nginx (Linux)

#### 1. Configuration du service systemd
```ini
# /etc/systemd/system/dior-enterprise.service
[Unit]
Description=Dior Enterprise Management System
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/dior-enterprise/Dior.Service.Host.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dior-enterprise
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000
WorkingDirectory=/var/www/dior-enterprise

[Install]
WantedBy=multi-user.target
```

#### 2. Configuration Nginx
```nginx
# /etc/nginx/sites-available/dior-enterprise
server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com;
    
    ssl_certificate /path/to/your/certificate.crt;
    ssl_certificate_key /path/to/your/private.key;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # Timeout settings
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Static files caching
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;
}
```

### ? V�rifications post-d�ploiement

#### 1. Tests de sant�
```bash
# Tester l'endpoint de sant�
curl -k https://yourdomain.com/health

# R�ponse attendue:
{
  "status": "healthy",
  "timestamp": "2025-01-27T10:30:00Z",
  "version": "1.0.0",
  "environment": "Production",
  "database": "Connected"
}
```

#### 2. Tests d'authentification
```bash
# Test de login
curl -X POST "https://yourdomain.com/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{
       "username": "admin",
       "password": "YourProductionPassword"
     }'

# V�rifier que le token JWT est retourn�
```

#### 3. Tests des endpoints principaux
```bash
# Test avec token JWT
curl -X GET "https://yourdomain.com/api/Users" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Test Swagger UI (si activ� en production)
curl -k https://yourdomain.com/swagger
```

### ? Monitoring et maintenance

#### 1. Scripts de monitoring
```bash
#!/bin/bash
# monitor-dior-enterprise.sh
# V�rifier que l'application fonctionne

URL="https://yourdomain.com/health"
RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" $URL)

if [ $RESPONSE -eq 200 ]; then
    echo "? Dior Enterprise is healthy"
    exit 0
else
    echo "? Dior Enterprise is down (HTTP $RESPONSE)"
    # Red�marrer le service si n�cessaire
    systemctl restart dior-enterprise
    exit 1
fi
```

#### 2. Logs de production
```bash
# Consulter les logs (Linux)
journalctl -u dior-enterprise -f

# Consulter les logs (Windows avec IIS)
# V�rifier: C:\inetpub\logs\LogFiles\

# Logs applicatifs
tail -f /var/www/dior-enterprise/logs/dior-enterprise-*.log
```

#### 3. Sauvegarde automatique
```sql
-- Script de sauvegarde SQL Server (� automatiser)
BACKUP DATABASE [Dior.Database.Production] 
TO DISK = 'C:\Backups\Dior.Database.Production_Full_YYYYMMDD_HHMMSS.bak'
WITH FORMAT, INIT, COMPRESSION, STATS = 10;
```

### ? S�curit� production

#### 1. Configuration firewall
```bash
# Ouvrir seulement les ports n�cessaires
ufw allow 22      # SSH
ufw allow 80      # HTTP (redirection HTTPS)
ufw allow 443     # HTTPS
ufw deny 5000     # Application port (proxied par Nginx)

# Autoriser seulement depuis certaines IPs si possible
ufw allow from YOUR.ADMIN.IP.ADDRESS to any port 22
```

#### 2. Configuration SSL/TLS
```bash
# G�n�rer un certificat Let's Encrypt (gratuit)
certbot --nginx -d yourdomain.com

# V�rifier le certificat
openssl x509 -in /path/to/certificate.crt -text -noout
```

#### 3. Durcissement de la configuration
```json
// appsettings.Production.json - Section s�curit�
{
  "Security": {
    "RequireHttps": true,
    "EnableCors": false,  // D�sactiver si pas n�cessaire
    "MaxRequestSize": 10485760,  // 10MB
    "PasswordRequirements": {
      "MinLength": 12,
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": true
    }
  }
}
```

### ?? D�pannage courant

#### Probl�me: Application ne d�marre pas
```bash
# V�rifier les logs
journalctl -u dior-enterprise --no-pager

# V�rifier la configuration
dotnet --info
systemctl status dior-enterprise

# Tester manuellement
cd /var/www/dior-enterprise
sudo -u www-data dotnet Dior.Service.Host.dll
```

#### Probl�me: Erreur de base de donn�es
```bash
# Tester la connexion BD
sqlcmd -S YourServer -U DiorAppUser -P YourPassword -d Dior.Database.Production -Q "SELECT 1"

# V�rifier les permissions
SELECT 
    dp.state_desc,
    dp.permission_name,
    s.name AS principal_name,
    o.name AS object_name
FROM sys.database_permissions dp
LEFT JOIN sys.objects o ON dp.major_id = o.object_id  
LEFT JOIN sys.database_principals s ON dp.grantee_principal_id = s.principal_id
WHERE s.name = 'DiorAppUser'
```

#### Probl�me: JWT Token invalide
```bash
# V�rifier la configuration JWT
grep -r "Jwt" /var/www/dior-enterprise/appsettings*.json

# V�rifier les variables d'environnement
printenv | grep JWT
```

### ?? Performance et scaling

#### 1. Configuration IIS pour haute charge
```xml
<!-- web.config - Performance settings -->
<system.webServer>
  <aspNetCore processPath="dotnet" 
              arguments=".\Dior.Service.Host.dll" 
              stdoutLogEnabled="false" 
              stdoutLogFile=".\logs\stdout" 
              hostingModel="inprocess"
              requestTimeout="00:20:00"
              shutdownTimeLimit="10">
    <environmentVariables>
      <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
      <environmentVariable name="ASPNETCORE_URLS" value="http://*:5000" />
    </environmentVariables>
  </aspNetCore>
</system.webServer>
```

#### 2. Optimisation base de donn�es
```sql
-- Index de performance pour production
CREATE INDEX IX_USER_Active_Team ON [USER](IsActive, TeamId) INCLUDE (FirstName, LastName, Email);
CREATE INDEX IX_Messages_UserTeam_Date ON Messages(RecipientUserId, RecipientTeamId, SentAt DESC);
CREATE INDEX IX_Payslips_UserPeriod ON Payslips(UserId, Year DESC, Month DESC);

-- Statistiques et maintenance
UPDATE STATISTICS [USER];
UPDATE STATISTICS Messages;
UPDATE STATISTICS Payslips;
UPDATE STATISTICS Contract;
```

---

## ? Checklist finale de d�ploiement

- [ ] Base de donn�es cr��e et configur�e
- [ ] Application publi�e et test�e
- [ ] Variables d'environnement configur�es  
- [ ] SSL/HTTPS configur� et test�
- [ ] Monitoring et logs en place
- [ ] Sauvegardes automatiques configur�es
- [ ] Tests de charge effectu�s (si critique)
- [ ] Documentation mise � jour
- [ ] �quipe form�e sur la production

**?? Votre syst�me Dior Enterprise est maintenant en production !**

---

**Support** : En cas de probl�me, consultez les logs et la documentation, ou contactez l'�quipe de d�veloppement.