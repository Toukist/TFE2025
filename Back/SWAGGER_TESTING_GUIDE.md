# ?? Guide de test Swagger - Dior Enterprise API

## ?? Vue d'ensemble

Ce guide vous explique comment tester efficacement l'API Dior Enterprise directement avec **Swagger UI**, sans avoir besoin d'outils externes comme Postman.

## ?? Accès à Swagger UI

### URL de développement
```
https://localhost:7201/swagger
```

### Interface utilisateur
- **Documentation interactive** avec tous les endpoints
- **Test direct** des API calls
- **Authentification JWT** intégrée
- **Exemples de données** pré-remplis

## ?? Authentification avec Swagger

### Étape 1: Se connecter
1. **Ouvrez** l'endpoint `POST /api/auth/login`
2. **Cliquez** sur "Try it out"
3. **Utilisez** un compte de test :

#### Comptes de test disponibles

| Rôle | Username | Password | Capacités |
|------|----------|----------|-----------|
| **?? Admin** | `admin` | `admin123` | Accès complet au système |
| **????? Manager** | `manager1` | `manager123` | Gestion équipes et projets |
| **?? RH** | `rh1` | `rh123` | Contrats et fiches de paie |
| **?? Opérateur** | `operateur1` | `operateur123` | Consultation uniquement |

### Étape 2: Récupérer le token
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Réponse attendue:**
```json
{
  "user": {
    "id": 1,
    "username": "admin",
    "roles": ["Admin"]
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 28800
}
```

### Étape 3: Configurer l'authentification
1. **Copiez** le token de la réponse (sans guillemets)
2. **Cliquez** sur le bouton **?? Authorize** en haut de la page
3. **Collez** votre token dans le champ (le préfixe Bearer est automatique)
4. **Cliquez** sur **Authorize**

? **Vous êtes maintenant authentifié !** Un cadenas fermé ?? apparaît sur les endpoints protégés.

## ?? Scénarios de test par rôle

### ?? Tests Admin

#### 1. Gestion des utilisateurs
```http
GET /api/Users
- Liste tous les utilisateurs avec leurs rôles
```

#### 2. Création d'un utilisateur
```json
POST /api/Users
{
  "userName": "nouvel.utilisateur",
  "firstName": "Nouveau",
  "lastName": "Utilisateur",
  "email": "nouveau@dior-enterprise.com",
  "password": "Password123!",
  "isActive": true,
  "roleIds": [4]
}
```

### ????? Tests Manager

#### 1. Créer un projet
```json
POST /api/Projet
{
  "nom": "Migration Cloud 2025",
  "description": "Migration de l'infrastructure vers Azure",
  "teamId": 1,
  "type": "Equipe",
  "dateDebut": "2025-02-01T00:00:00Z",
  "dateFin": "2025-06-30T23:59:59Z"
}
```

#### 2. Message à l'équipe
```json
POST /api/Message/team
{
  "subject": "Réunion équipe urgente",
  "content": "Réunion demain à 14h en salle de conférence",
  "recipientTeamId": 1,
  "priority": "High",
  "messageType": "Announcement"
}
```

#### 3. Suivre mes projets
```http
GET /api/Projet/my
- Récupère tous vos projets en tant que manager
```

### ?? Tests RH

#### 1. Créer un contrat CDI
```json
POST /api/Contract
{
  "userId": 2,
  "contractType": "CDI",
  "startDate": "2025-02-01T00:00:00Z",
  "salary": 3500.00,
  "currency": "EUR",
  "paymentFrequency": "Mensuel",
  "fileName": "contrat-cdi-john-doe.pdf",
  "fileUrl": "/uploads/contracts/contrat-cdi-john-doe.pdf"
}
```

#### 2. Générer fiches de paie pour équipe
```json
POST /api/Payslip/generate
{
  "month": 1,
  "year": 2025,
  "teamId": 1
}
```

#### 3. Consulter les contrats
```http
GET /api/Contract
- Liste tous les contrats de l'entreprise
```

### ?? Tests Opérateur

#### 1. Consulter mes documents
```http
GET /api/Contract/my
- Mes contrats personnels
```

```http
GET /api/Payslip/my
- Mes fiches de paie
```

#### 2. Mes messages
```http
GET /api/Message/my
- Messages reçus
```

## ?? Endpoints de monitoring

### Health Check
```http
GET /health
- Statut du système (sans authentification)
```

### Informations API
```http
GET /
- Informations générales sur l'API
```

### Mon profil
```http
GET /api/auth/me
- Informations sur l'utilisateur connecté
```

## ?? Conseils pour les tests

### ?? Lecture des réponses
- **200-299** : Succès ?
- **400-499** : Erreur client (données invalides, droits insuffisants) ??
- **500-599** : Erreur serveur ??

### ?? Données de test
- Les **IDs** commencent généralement à 1
- Les **dates** doivent être au format ISO 8601 : `YYYY-MM-DDTHH:mm:ssZ`
- Les **emails** doivent être valides
- Les **TeamId** existants : 1, 2, 3...

### ?? Gestion des permissions
- **403 Forbidden** = Rôle insuffisant pour cette action
- **401 Unauthorized** = Token manquant ou expiré
- **404 Not Found** = Ressource inexistante

### ? Expiration du token
- **Durée** : 8 heures
- **Renouvellement** : Refaites un login si expiré
- **Validation** : Utilisez `GET /api/auth/validate`

## ?? Workflow de test complet

### 1. Test de connexion
```http
POST /api/auth/login
Body: {"username": "admin", "password": "admin123"}
```

### 2. Configuration auth
- Copier le token
- Authorize dans Swagger UI

### 3. Tests fonctionnels
- Tester les endpoints selon votre rôle
- Vérifier les codes de réponse
- Examiner les données retournées

### 4. Tests d'erreurs
- Essayer sans authentification
- Tester avec un rôle insuffisant
- Envoyer des données invalides

## ?? Ressources supplémentaires

### Documentation Swagger
- **Modèles** : Cliquez sur "Schemas" en bas
- **Exemples** : Pré-remplis dans chaque endpoint
- **Descriptions** : Détails sur chaque paramètre

### Raccourcis utiles
- **Ctrl+F** : Recherche dans la documentation
- **Expand Operations** : Voir tous les endpoints
- **Try it out** : Activer le test d'un endpoint
- **Execute** : Lancer l'appel API

---

## ?? Objectifs des tests

? **Authentification fonctionnelle**  
? **Autorisation par rôle respectée**  
? **CRUD operations complètes**  
? **Gestion d'erreurs appropriée**  
? **Données cohérentes**  

**?? Bonne utilisation de l'API Dior Enterprise avec Swagger !**