# ?? Dior Enterprise Management System

## ?? Vue d'ensemble

Le **Dior Enterprise Management System** est une API REST compl�te d�velopp�e en **.NET 8** pour la gestion d'entreprise avec quatre r�les m�tiers distincts et leurs workflows sp�cifiques.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-yellow)
![License](https://img.shields.io/badge/license-MIT-green)

## ?? Fonctionnalit�s principales

### ?? Syst�me de r�les � 4 niveaux
- **?? ADMIN** : Gestion compl�te du syst�me, utilisateurs et privil�ges
- **????? MANAGER** : Gestion d'�quipe, projets et messagerie
- **?? RH** : Gestion des contrats, fiches de paie et �valuations 
- **?? OP�RATEUR** : Consultation des t�ches et documents personnels

### ?? Modules impl�ment�s
- ? **Gestion des utilisateurs** avec authentification JWT
- ? **Gestion d'�quipes** et hi�rarchie organisationnelle
- ? **Gestion de projets** avec suivi de progression
- ? **Syst�me de messagerie** interne (�quipe/individuel)
- ? **Gestion des contrats** RH avec diff�rents types
- ? **G�n�ration de fiches de paie** automatis�e
- ? **Syst�me de permissions** granulaire
- ? **Audit trail** complet des actions

## ??? Architecture

### Structure du projet
```
Dior.Enterprise/
??? ?? Dior.Library/          # Mod�les, DTOs et interfaces
?   ??? BO/                   # Business Objects
?   ??? DTO/                  # Data Transfer Objects
?   ??? DAO/                  # Data Access Objects interfaces
?   ??? Exceptions/           # Exceptions personnalis�es
??? ?? Dior.Service/          # Services m�tiers
?   ??? Services/             # Impl�mentations des services
?   ??? DAO/                  # Acc�s aux donn�es
?   ??? Mappers/              # Conversion d'objets
??? ?? Dior.Service.Host/     # API Web
    ??? Controllers/          # Contr�leurs REST
    ??? Middleware/           # Middleware personnalis�s
    ??? Services/             # Services de l'h�te (JWT, etc.)
```

### Technologies utilis�es
- **Backend** : ASP.NET Core 8.0 Web API
- **Base de donn�es** : SQL Server 2019+
- **Authentification** : JWT Bearer Tokens
- **Documentation** : Swagger/OpenAPI
- **ORM** : Entity Framework Core + ADO.NET
- **S�curit�** : BCrypt pour les mots de passe

## ?? Installation et configuration

### Pr�requis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) ou SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code

### 1. Cloner le projet
```bash
git clone <repository-url>
cd Dior.Enterprise
```

### 2. Configuration de la base de donn�es

#### A. Modifier la cha�ne de connexion
�ditez `Dior.Service.Host/appsettings.json` :
```json
{
  "ConnectionStrings": {
    "Dior_DB": "Server=.;Database=Dior.Database;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### B. Cr�er la base de donn�es et les tables
1. Cr�ez la base de donn�es `Dior.Database` dans SQL Server
2. Ex�cutez le script `CreateMissingTables.sql` :
```sql
-- Le script est fourni et cr�e toutes les tables n�cessaires
-- avec gestion d'erreurs et contraintes de validation
```

### 3. Configuration JWT
Modifiez la cl� secr�te JWT dans `appsettings.json` :
```json
{
  "Jwt": {
    "Secret": "VotreCleSecrete256BitsMinimumPourJWT2025SecuriteMaximale!",
    "Issuer": "DiorEnterpriseAPI",
    "Audience": "DiorEnterpriseClient", 
    "ExpirationMinutes": 480
  }
}
```

### 4. Compilation et lancement
```bash
# Restaurer les packages
dotnet restore

# Compiler
dotnet build

# Lancer l'API
cd Dior.Service.Host
dotnet run
```

## ?? API Documentation

### Endpoints principaux

#### ?? Authentification
```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```

#### ?? Gestion des utilisateurs
```http
GET /api/Users                    # Liste des utilisateurs
GET /api/Users/{id}               # D�tails d'un utilisateur  
GET /api/Users/{id}/full          # Infos compl�tes avec �quipe
GET /api/Users/my-team            # �quipe du manager (Manager only)
POST /api/Users                   # Cr�er un utilisateur
PUT /api/Users/{id}               # Modifier un utilisateur
DELETE /api/Users/{id}            # Supprimer un utilisateur
```

#### ?? Gestion des projets
```http
GET /api/Projet                   # Tous les projets
GET /api/Projet/{id}              # D�tails d'un projet
GET /api/Projet/my                # Mes projets (Manager)
GET /api/Projet/team/{teamId}     # Projets d'une �quipe
POST /api/Projet                  # Cr�er un projet (Manager/Admin)
PUT /api/Projet/{id}              # Modifier un projet
PATCH /api/Projet/{id}/progress   # Mettre � jour le progr�s
DELETE /api/Projet/{id}           # Supprimer un projet
```

#### ?? Syst�me de messagerie  
```http
GET /api/Message/my               # Mes messages
GET /api/Message/unread-count     # Nombre de messages non lus
POST /api/Message/team            # Message � l'�quipe (Manager)
POST /api/Message/user            # Message individuel
PATCH /api/Message/{id}/read      # Marquer comme lu
```

#### ?? Gestion des contrats (RH)
```http
GET /api/Contract                 # Tous les contrats (RH/Admin)
GET /api/Contract/my              # Mes contrats (Employ�)
GET /api/Contract/user/{userId}   # Contrats d'un utilisateur (RH)
POST /api/Contract                # Cr�er un contrat (RH)
PUT /api/Contract/{id}            # Modifier un contrat (RH)
PATCH /api/Contract/{id}/terminate # Terminer un contrat (RH)
```

#### ?? Gestion des fiches de paie (RH)
```http
GET /api/Payslip                  # Toutes les fiches (RH/Admin)
GET /api/Payslip/my               # Mes fiches de paie (Employ�)
GET /api/Payslip/period/{year}/{month} # Fiches par p�riode (RH)
POST /api/Payslip/generate        # G�n�rer les fiches (RH)
POST /api/Payslip/{id}/send       # Envoyer une fiche (RH)
POST /api/Payslip/send-bulk       # Envoi group� (RH)
```

### ?? Authentification et autorisation

#### Utilisation du JWT
1. **Login** : `POST /api/Auth/login` avec credentials
2. **Token retourn�** : Utilisez le token dans le header Authorization
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### R�les et permissions
- **Admin** : Acc�s total au syst�me
- **Manager** : Gestion de son �quipe et ses projets
- **RH** : Gestion des employ�s, contrats et paies
- **Op�rateur** : Consultation de ses donn�es personnelles

## ?? Tests avec Swagger

### Acc�s � Swagger UI
- **URL** : `https://localhost:7201/swagger`
- **Production** : Authentification Basic (admin/admin2024)

### Sc�narios de test

#### 1. Test complet Manager
```http
# 1. Login en tant que Manager
POST /api/Auth/login
{
  "username": "manager1", 
  "password": "password123"
}

# 2. Cr�er un projet pour son �quipe
POST /api/Projet
Authorization: Bearer {token}
{
  "nom": "Migration Cloud Azure",
  "description": "Migration compl�te vers Azure",
  "type": "Equipe",
  "dateDebut": "2025-02-01",
  "dateFin": "2025-06-30"
}

# 3. Envoyer message � l'�quipe
POST /api/Message/team  
Authorization: Bearer {token}
{
  "subject": "R�union �quipe",
  "content": "R�union demain � 14h en salle de conf�rence",
  "priority": "High",
  "messageType": "Announcement",
  "recipientTeamId": 1
}
```

#### 2. Test complet RH
```http
# 1. Login en tant que RH
POST /api/Auth/login
{
  "username": "rh1",
  "password": "password123" 
}

# 2. Cr�er un contrat CDI
POST /api/Contract
Authorization: Bearer {token}
{
  "userId": 5,
  "contractType": "CDI",
  "startDate": "2025-02-01", 
  "salary": 3500.00,
  "currency": "EUR",
  "paymentFrequency": "Mensuel"
}

# 3. G�n�rer fiches de paie pour l'�quipe
POST /api/Payslip/generate
Authorization: Bearer {token}
{
  "month": 1,
  "year": 2025,
  "teamId": 1
}
```

## ??? Base de donn�es

### Tables principales
- **USER** : Utilisateurs et authentification
- **Team** : �quipes organisationnelles  
- **Projet** : Projets avec manager et progression
- **Messages** : Syst�me de messagerie interne
- **Contract** : Contrats employ�s avec d�tails RH
- **Payslips** : Fiches de paie g�n�r�es
- **RoleDefinition** : D�finition des r�les syst�me
- **UserRole** : Attribution des r�les aux utilisateurs

### Relations cl�s
```sql
USER ?? Team (N:1)
USER ?? Projet (Manager 1:N) 
USER ?? Messages (Sender/Recipient N:N)
USER ?? Contract (1:N)
USER ?? Payslips (1:N)
USER ?? UserRole ?? RoleDefinition (N:N)
```

## ?? Configuration avanc�e

### Variables d'environnement
```bash
# JWT Configuration
Jwt__Secret=VotreCleSecreteSecure256Bits!
Jwt__ExpirationMinutes=480

# Database
ConnectionStrings__Dior_DB=Server=.;Database=Dior.Database;...

# Email (pour envoi fiches de paie)
Email__SmtpHost=smtp.gmail.com
Email__SmtpUser=notifications@company.com
```

### Profils de lancement
```json
// launchSettings.json
{
  "profiles": {
    "Development": {
      "commandName": "Project", 
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:7201;http://localhost:5201"
    }
  }
}
```

## ?? Monitoring et logs

### Logs structur�s
Le syst�me utilise Serilog pour les logs structur�s :
```csharp
// Logs dans console et fichier
// Fichiers : logs/dior-enterprise-{Date}.log
// R�tention : 7 jours
```

### Health checks
```http
GET /health
{
  "status": "healthy",
  "timestamp": "2025-01-27T10:30:00Z",
  "version": "1.0.0",
  "environment": "Development",
  "database": "Connected"
}
```

## ?? S�curit�

### Mesures impl�ment�es
- ? **JWT tokens** avec expiration
- ? **Hachage BCrypt** des mots de passe  
- ? **Autorisation par r�les** granulaire
- ? **Validation des entr�es** stricte
- ? **HTTPS obligatoire** en production
- ? **CORS configur�** pour Angular
- ? **Audit trail** des actions sensibles

### Recommandations production
- Changer la cl� JWT secr�te
- Configurer HTTPS avec certificat valide
- Activer la limitation de d�bit (rate limiting)
- Configurer les logs de s�curit�
- Sauvegardes automatiques de la BD

## ?? Contribution

### Standards de code
- ? **Conventions C#** : PascalCase pour classes/m�thodes
- ? **Async/await** partout o� c'est appropri�
- ? **Documentation XML** sur m�thodes publiques
- ? **Gestion d'erreurs** avec try/catch et middleware
- ? **Tests unitaires** recommand�s (TODO)

### Workflow de d�veloppement
1. Fork le projet
2. Cr�er une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commiter les changements (`git commit -m 'Add AmazingFeature'`)  
4. Pousser la branche (`git push origin feature/AmazingFeature`)
5. Cr�er une Pull Request

## ?? Support

### R�solution de probl�mes

#### Erreurs communes
1. **Erreur connection BD** : V�rifiez la cha�ne de connexion
2. **JWT Invalid** : V�rifiez la cl� secr�te et l'expiration
3. **403 Forbidden** : V�rifiez les r�les de l'utilisateur
4. **Build errors** : `dotnet clean && dotnet restore && dotnet build`

#### Logs utiles
```bash
# V�rifier les logs
tail -f logs/dior-enterprise-*.log

# V�rifier la base de donn�es
SELECT * FROM [USER] WHERE IsActive = 1
SELECT * FROM Messages WHERE IsRead = 0
```

## ?? Roadmap

### Version 1.1 (Q2 2025)
- [ ] Dashboard temps r�el avec SignalR
- [ ] API GraphQL en compl�ment REST
- [ ] Tests unitaires complets
- [ ] Int�gration continue (CI/CD)

### Version 1.2 (Q3 2025)  
- [ ] Module de formation en ligne
- [ ] �valuations de performance automatis�es
- [ ] Reporting avanc� avec export Excel/PDF
- [ ] Application mobile (Xamarin/MAUI)

### Version 2.0 (Q4 2025)
- [ ] Microservices architecture
- [ ] Int�gration avec syst�mes externes (ERP/CRM)
- [ ] IA pour recommandations RH
- [ ] Multi-tenant pour plusieurs entreprises

---

## ?? Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de d�tails.

## ? Remerciements

- **ASP.NET Core Team** pour le framework excellent
- **Entity Framework Team** pour l'ORM robuste  
- **JWT.NET** pour l'authentification s�curis�e
- **Swagger** pour la documentation API automatique

---

**?? Dior Enterprise Management System v1.0**  
*D�velopp� avec ?? pour la gestion d'entreprise moderne*