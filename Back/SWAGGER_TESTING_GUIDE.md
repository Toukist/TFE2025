# ?? Guide de test Swagger - Dior Enterprise API

## ?? Vue d'ensemble

Ce guide vous explique comment tester efficacement l'API Dior Enterprise directement avec **Swagger UI**, sans avoir besoin d'outils externes comme Postman.

## ?? Acc�s � Swagger UI

### URL de d�veloppement
```
https://localhost:7201/swagger
```

### Interface utilisateur
- **Documentation interactive** avec tous les endpoints
- **Test direct** des API calls
- **Authentification JWT** int�gr�e
- **Exemples de donn�es** pr�-remplis

## ?? Authentification avec Swagger

### �tape 1: Se connecter
1. **Ouvrez** l'endpoint `POST /api/auth/login`
2. **Cliquez** sur "Try it out"
3. **Utilisez** un compte de test :

#### Comptes de test disponibles

| R�le | Username | Password | Capacit�s |
|------|----------|----------|-----------|
| **?? Admin** | `admin` | `admin123` | Acc�s complet au syst�me |
| **????? Manager** | `manager1` | `manager123` | Gestion �quipes et projets |
| **?? RH** | `rh1` | `rh123` | Contrats et fiches de paie |
| **?? Op�rateur** | `operateur1` | `operateur123` | Consultation uniquement |

### �tape 2: R�cup�rer le token
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**R�ponse attendue:**
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

### �tape 3: Configurer l'authentification
1. **Copiez** le token de la r�ponse (sans guillemets)
2. **Cliquez** sur le bouton **?? Authorize** en haut de la page
3. **Collez** votre token dans le champ (le pr�fixe Bearer est automatique)
4. **Cliquez** sur **Authorize**

? **Vous �tes maintenant authentifi� !** Un cadenas ferm� ?? appara�t sur les endpoints prot�g�s.

## ?? Sc�narios de test par r�le

### ?? Tests Admin

#### 1. Gestion des utilisateurs
```http
GET /api/Users
- Liste tous les utilisateurs avec leurs r�les
```

#### 2. Cr�ation d'un utilisateur
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

#### 1. Cr�er un projet
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

#### 2. Message � l'�quipe
```json
POST /api/Message/team
{
  "subject": "R�union �quipe urgente",
  "content": "R�union demain � 14h en salle de conf�rence",
  "recipientTeamId": 1,
  "priority": "High",
  "messageType": "Announcement"
}
```

#### 3. Suivre mes projets
```http
GET /api/Projet/my
- R�cup�re tous vos projets en tant que manager
```

### ?? Tests RH

#### 1. Cr�er un contrat CDI
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

#### 2. G�n�rer fiches de paie pour �quipe
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

### ?? Tests Op�rateur

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
- Messages re�us
```

## ?? Endpoints de monitoring

### Health Check
```http
GET /health
- Statut du syst�me (sans authentification)
```

### Informations API
```http
GET /
- Informations g�n�rales sur l'API
```

### Mon profil
```http
GET /api/auth/me
- Informations sur l'utilisateur connect�
```

## ?? Conseils pour les tests

### ?? Lecture des r�ponses
- **200-299** : Succ�s ?
- **400-499** : Erreur client (donn�es invalides, droits insuffisants) ??
- **500-599** : Erreur serveur ??

### ?? Donn�es de test
- Les **IDs** commencent g�n�ralement � 1
- Les **dates** doivent �tre au format ISO 8601 : `YYYY-MM-DDTHH:mm:ssZ`
- Les **emails** doivent �tre valides
- Les **TeamId** existants : 1, 2, 3...

### ?? Gestion des permissions
- **403 Forbidden** = R�le insuffisant pour cette action
- **401 Unauthorized** = Token manquant ou expir�
- **404 Not Found** = Ressource inexistante

### ? Expiration du token
- **Dur�e** : 8 heures
- **Renouvellement** : Refaites un login si expir�
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
- Tester les endpoints selon votre r�le
- V�rifier les codes de r�ponse
- Examiner les donn�es retourn�es

### 4. Tests d'erreurs
- Essayer sans authentification
- Tester avec un r�le insuffisant
- Envoyer des donn�es invalides

## ?? Ressources suppl�mentaires

### Documentation Swagger
- **Mod�les** : Cliquez sur "Schemas" en bas
- **Exemples** : Pr�-remplis dans chaque endpoint
- **Descriptions** : D�tails sur chaque param�tre

### Raccourcis utiles
- **Ctrl+F** : Recherche dans la documentation
- **Expand Operations** : Voir tous les endpoints
- **Try it out** : Activer le test d'un endpoint
- **Execute** : Lancer l'appel API

---

## ?? Objectifs des tests

? **Authentification fonctionnelle**  
? **Autorisation par r�le respect�e**  
? **CRUD operations compl�tes**  
? **Gestion d'erreurs appropri�e**  
? **Donn�es coh�rentes**  

**?? Bonne utilisation de l'API Dior Enterprise avec Swagger !**