# ?? Dior Enterprise Management System

## ?? Vue d'ensemble

Le **Dior Enterprise Management System** est une API REST complète développée en **.NET 8** pour la gestion d'entreprise avec quatre rôles métiers distincts et leurs workflows spécifiques.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-yellow)
![License](https://img.shields.io/badge/license-MIT-green)

## ?? Fonctionnalités principales

### ?? Système de rôles à 4 niveaux
- **?? ADMIN** : Gestion complète du système, utilisateurs et privilèges
- **????? MANAGER** : Gestion d'équipe, projets et messagerie
- **?? RH** : Gestion des contrats, fiches de paie et évaluations 
- **?? OPÉRATEUR** : Consultation des tâches et documents personnels

### ?? Modules implémentés
- ? **Gestion des utilisateurs** avec authentification JWT
- ? **Gestion d'équipes** et hiérarchie organisationnelle
- ? **Gestion de projets** avec suivi de progression
- ? **Système de messagerie** interne (équipe/individuel)
- ? **Gestion des contrats** RH avec différents types
- ? **Génération de fiches de paie** automatisée
- ? **Système de permissions** granulaire
- ? **Audit trail** complet des actions

## ??? Architecture

### Structure du projet
```
Dior.Enterprise/
??? ?? Dior.Library/          # Modèles, DTOs et interfaces
?   ??? BO/                   # Business Objects
?   ??? DTO/                  # Data Transfer Objects
?   ??? DAO/                  # Data Access Objects interfaces
?   ??? Exceptions/           # Exceptions personnalisées
??? ?? Dior.Service/          # Services métiers
?   ??? Services/             # Implémentations des services
?   ??? DAO/                  # Accès aux données
?   ??? Mappers/              # Conversion d'objets
??? ?? Dior.Service.Host/     # API Web
    ??? Controllers/          # Contrôleurs REST
    ??? Middleware/           # Middleware personnalisés
    ??? Services/             # Services de l'hôte (JWT, etc.)
```

### Technologies utilisées
- **Backend** : ASP.NET Core 8.0 Web API
- **Base de données** : SQL Server 2019+
- **Authentification** : JWT Bearer Tokens
- **Documentation** : Swagger/OpenAPI
- **ORM** : Entity Framework Core + ADO.NET
- **Sécurité** : BCrypt pour les mots de passe

## ?? Installation et configuration

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) ou SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code

### 1. Cloner le projet
```bash
git clone <repository-url>
cd Dior.Enterprise
```

### 2. Configuration de la base de données

#### A. Modifier la chaîne de connexion
Éditez `Dior.Service.Host/appsettings.json` :
```json
{
  "ConnectionStrings": {
    "Dior_DB": "Server=.;Database=Dior.Database;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### B. Créer la base de données et les tables
1. Créez la base de données `Dior.Database` dans SQL Server
2. Exécutez le script `CreateMissingTables.sql` :
```sql
-- Le script est fourni et crée toutes les tables nécessaires
-- avec gestion d'erreurs et contraintes de validation
```

### 3. Configuration JWT
Modifiez la clé secrète JWT dans `appsettings.json` :
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
GET /api/Users/{id}               # Détails d'un utilisateur  
GET /api/Users/{id}/full          # Infos complètes avec équipe
GET /api/Users/my-team            # Équipe du manager (Manager only)
POST /api/Users                   # Créer un utilisateur
PUT /api/Users/{id}               # Modifier un utilisateur
DELETE /api/Users/{id}            # Supprimer un utilisateur
```

#### ?? Gestion des projets
```http
GET /api/Projet                   # Tous les projets
GET /api/Projet/{id}              # Détails d'un projet
GET /api/Projet/my                # Mes projets (Manager)
GET /api/Projet/team/{teamId}     # Projets d'une équipe
POST /api/Projet                  # Créer un projet (Manager/Admin)
PUT /api/Projet/{id}              # Modifier un projet
PATCH /api/Projet/{id}/progress   # Mettre à jour le progrès
DELETE /api/Projet/{id}           # Supprimer un projet
```

#### ?? Système de messagerie  
```http
GET /api/Message/my               # Mes messages
GET /api/Message/unread-count     # Nombre de messages non lus
POST /api/Message/team            # Message à l'équipe (Manager)
POST /api/Message/user            # Message individuel
PATCH /api/Message/{id}/read      # Marquer comme lu
```

#### ?? Gestion des contrats (RH)
```http
GET /api/Contract                 # Tous les contrats (RH/Admin)
GET /api/Contract/my              # Mes contrats (Employé)
GET /api/Contract/user/{userId}   # Contrats d'un utilisateur (RH)
POST /api/Contract                # Créer un contrat (RH)
PUT /api/Contract/{id}            # Modifier un contrat (RH)
PATCH /api/Contract/{id}/terminate # Terminer un contrat (RH)
```

#### ?? Gestion des fiches de paie (RH)
```http
GET /api/Payslip                  # Toutes les fiches (RH/Admin)
GET /api/Payslip/my               # Mes fiches de paie (Employé)
GET /api/Payslip/period/{year}/{month} # Fiches par période (RH)
POST /api/Payslip/generate        # Générer les fiches (RH)
POST /api/Payslip/{id}/send       # Envoyer une fiche (RH)
POST /api/Payslip/send-bulk       # Envoi groupé (RH)
```

### ?? Authentification et autorisation

#### Utilisation du JWT
1. **Login** : `POST /api/Auth/login` avec credentials
2. **Token retourné** : Utilisez le token dans le header Authorization
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Rôles et permissions
- **Admin** : Accès total au système
- **Manager** : Gestion de son équipe et ses projets
- **RH** : Gestion des employés, contrats et paies
- **Opérateur** : Consultation de ses données personnelles

## ?? Tests avec Swagger

### Accès à Swagger UI
- **URL** : `https://localhost:7201/swagger`
- **Production** : Authentification Basic (admin/admin2024)

### Scénarios de test

#### 1. Test complet Manager
```http
# 1. Login en tant que Manager
POST /api/Auth/login
{
  "username": "manager1", 
  "password": "password123"
}

# 2. Créer un projet pour son équipe
POST /api/Projet
Authorization: Bearer {token}
{
  "nom": "Migration Cloud Azure",
  "description": "Migration complète vers Azure",
  "type": "Equipe",
  "dateDebut": "2025-02-01",
  "dateFin": "2025-06-30"
}

# 3. Envoyer message à l'équipe
POST /api/Message/team  
Authorization: Bearer {token}
{
  "subject": "Réunion équipe",
  "content": "Réunion demain à 14h en salle de conférence",
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

# 2. Créer un contrat CDI
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

# 3. Générer fiches de paie pour l'équipe
POST /api/Payslip/generate
Authorization: Bearer {token}
{
  "month": 1,
  "year": 2025,
  "teamId": 1
}
```

## ??? Base de données

### Tables principales
- **USER** : Utilisateurs et authentification
- **Team** : Équipes organisationnelles  
- **Projet** : Projets avec manager et progression
- **Messages** : Système de messagerie interne
- **Contract** : Contrats employés avec détails RH
- **Payslips** : Fiches de paie générées
- **RoleDefinition** : Définition des rôles système
- **UserRole** : Attribution des rôles aux utilisateurs

### Relations clés
```sql
USER ?? Team (N:1)
USER ?? Projet (Manager 1:N) 
USER ?? Messages (Sender/Recipient N:N)
USER ?? Contract (1:N)
USER ?? Payslips (1:N)
USER ?? UserRole ?? RoleDefinition (N:N)
```

## ?? Configuration avancée

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

### Logs structurés
Le système utilise Serilog pour les logs structurés :
```csharp
// Logs dans console et fichier
// Fichiers : logs/dior-enterprise-{Date}.log
// Rétention : 7 jours
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

## ?? Sécurité

### Mesures implémentées
- ? **JWT tokens** avec expiration
- ? **Hachage BCrypt** des mots de passe  
- ? **Autorisation par rôles** granulaire
- ? **Validation des entrées** stricte
- ? **HTTPS obligatoire** en production
- ? **CORS configuré** pour Angular
- ? **Audit trail** des actions sensibles

### Recommandations production
- Changer la clé JWT secrète
- Configurer HTTPS avec certificat valide
- Activer la limitation de débit (rate limiting)
- Configurer les logs de sécurité
- Sauvegardes automatiques de la BD

## ?? Contribution

### Standards de code
- ? **Conventions C#** : PascalCase pour classes/méthodes
- ? **Async/await** partout où c'est approprié
- ? **Documentation XML** sur méthodes publiques
- ? **Gestion d'erreurs** avec try/catch et middleware
- ? **Tests unitaires** recommandés (TODO)

### Workflow de développement
1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commiter les changements (`git commit -m 'Add AmazingFeature'`)  
4. Pousser la branche (`git push origin feature/AmazingFeature`)
5. Créer une Pull Request

## ?? Support

### Résolution de problèmes

#### Erreurs communes
1. **Erreur connection BD** : Vérifiez la chaîne de connexion
2. **JWT Invalid** : Vérifiez la clé secrète et l'expiration
3. **403 Forbidden** : Vérifiez les rôles de l'utilisateur
4. **Build errors** : `dotnet clean && dotnet restore && dotnet build`

#### Logs utiles
```bash
# Vérifier les logs
tail -f logs/dior-enterprise-*.log

# Vérifier la base de données
SELECT * FROM [USER] WHERE IsActive = 1
SELECT * FROM Messages WHERE IsRead = 0
```

## ?? Roadmap

### Version 1.1 (Q2 2025)
- [ ] Dashboard temps réel avec SignalR
- [ ] API GraphQL en complément REST
- [ ] Tests unitaires complets
- [ ] Intégration continue (CI/CD)

### Version 1.2 (Q3 2025)  
- [ ] Module de formation en ligne
- [ ] Évaluations de performance automatisées
- [ ] Reporting avancé avec export Excel/PDF
- [ ] Application mobile (Xamarin/MAUI)

### Version 2.0 (Q4 2025)
- [ ] Microservices architecture
- [ ] Intégration avec systèmes externes (ERP/CRM)
- [ ] IA pour recommandations RH
- [ ] Multi-tenant pour plusieurs entreprises

---

## ?? Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## ? Remerciements

- **ASP.NET Core Team** pour le framework excellent
- **Entity Framework Team** pour l'ORM robuste  
- **JWT.NET** pour l'authentification sécurisée
- **Swagger** pour la documentation API automatique

---

**?? Dior Enterprise Management System v1.0**  
*Développé avec ?? pour la gestion d'entreprise moderne*