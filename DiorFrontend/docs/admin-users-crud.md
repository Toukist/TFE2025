# Dossier Technique Complet – Module Admin Utilisateurs (Angular 20 Standalone)

Date: 17/08/2025  
Projet: DiorFrontend  
Version Angular: 20 (standalone, functional interceptors)

---
## 1. Objectif
Fournir un CRUD complet côté Admin pour la gestion des utilisateurs :
- Liste des utilisateurs (lecture)
- Création
- Édition
- Suppression
- Affichage d’attributs enrichis (équipe, statut, rôles)

Intégration avec backend exposant des endpoints REST `/api/User` (proxy dev) + JWT.

---
## 2. Architecture & Localisation des fichiers
| Aspect | Fichier | Rôle |
|--------|---------|------|
| Modèles | `src/app/models/user.model.ts` | Interfaces `UserDto`, `CreateUserDto`, `UpdateUserDto`, fusion “Full” + UI |
| Service API | `src/app/services/user.service.ts` | Appels REST utilisateurs |
| Composant liste admin (legacy routing) | `src/app/admin/user-full-list.component.ts` | Liste (ancienne version – encore référencée dans routes) |
| Nouveau composant liste + orchestration | `src/app/components/admin-users/admin-users.component.ts` | Liste + intégration formulaire inline |
| Formulaire create / edit | `src/app/components/admin-users/user-form.component.ts` | Formulaire (standalone) |
| Routes globales | `src/app/app.routes.ts` | Déclarations `admin/users` (actuellement vers *user-full-list*) |
| Intercepteur JWT | `src/app/interceptors/auth.interceptor.ts` | Ajout `Authorization: Bearer <token>` |
| Environnement | `src/environments/environment.ts` | `apiUrl: '/api'` (proxy) |
| Proxy dev | `proxy.conf.json` | Redirection `/api/*` -> `https://localhost:7201` |
| UI (Material ponctuel) | imports locaux dans composants | Pas de module agrégateur |

---
## 3. Modèle de données (extrait clef)
```ts
export interface UserDto {
  id: number; userId?: number; isActive: boolean;
  firstName: string; lastName: string; email: string;
  userName?: string; phone?: string; teamId?: number|null; teamName?: string|null;
  badgePhysicalNumber?: number|null; roles?: RoleDefinitionDto[]|null;
  name?: string; Name?: string; // compat
  createdAt?: string|Date; updatedAt?: string|Date; lastEditAt?: string|Date|null; lastEditBy?: string;
  avatar?: string; status?: string; lastConnection?: string|Date; role?: string;
  accessCompetencies?: any[]; password?: string;
}
```
Notes:
- Fusion de l’ancien `UserFullDto` dans `UserDto` (champs optionnels UI).
- `userId` présent pour endpoints envoyant deux identifiants.
- Champs UI non persistés côté backend (avatar, status, lastConnection).

---
## 4. Service API
Base URL: `environment.apiUrl + '/User'` → `/api/User` (proxy).  
Méthodes :
- `getAll()` → GET `/User`
- `getAllFull()` → GET `/User/full` (À VALIDER : si l’endpoint n’existe pas, remplacer par `getAll()`)
- `getById(id)` → GET `/User/{id}`
- `getMe()` → GET `/User/me`
- `create(dto)` → POST `/User`
- `update(id,dto)` → PUT `/User/{id}`
- `delete(id)` → DELETE `/User/{id}`

Recommandation: si le backend ne fournit pas `/User/full`, supprimer / renommer `getAllFull` et ajuster composants.

---
## 5. Intercepteur JWT
Intercepteur fonctionnel (`HttpInterceptorFn`) :
- Ignore route contenant `/auth/login`.
- Lit `authToken` depuis `localStorage`.
- Clone la requête avec header `Authorization`.

Vérifier: rafraîchissement de token non géré (à implémenter si besoin).

---
## 6. Routage Admin (état actuel)
Dans `app.routes.ts` la route `/admin/users` charge `user-full-list.component`.  
Nouveau composant cible souhaité: `AdminUsersComponent`.

Changement proposé :
```ts
{
  path: 'admin/users',
  loadComponent: () => import('./components/admin-users/admin-users.component').then(m => m.AdminUsersComponent),
  canActivate: [authGuard, RoleGuard],
  data: { roles: ['admin','administrateur'] },
  title: 'Gestion des utilisateurs - Admin'
}
```
Supprimer ensuite les routes spécifiques `user-full-*` si obsolètes.

---
## 7. Composant AdminUsers (résumé logique)
Responsabilités :
- Récupérer utilisateurs + accessCompetencies (forkJoin)
- Gérer états UI: loading, error, form visible, édition
- Orchestrer création, mise à jour et suppression
- Synchroniser access competencies (méthodes privées d’update / create)

Points d’amélioration :
- Extraire la logique d’accès aux accessCompetencies dans un service dédié ou facade.
- Normaliser le modèle AccessCompetency (supprimer any).
- Ajouter feedback (snackbar) plutôt que alert.

---
## 8. Formulaire UserForm (résumé)
Champs gérés: `lastName, firstName, name, email, phone, teamId, password, roleIds, accessCompetencyIds`.
Émissions: `save(UserDto)`, `cancel()`.
Validation: required sur nom, prénom, identifiant, email.
À ajouter potentiellement: pattern email renforcé, validation force mdp si création.

---
## 9. Séquence CRUD (proposé)
1. Liste initiale: `AdminUsersComponent.loadData()` → `getFullUsers()` (à remplacer si endpoint absent).
2. Création: ouvre formulaire → `userService.create()` → rechargement liste → mapping accessCompetencies si nécessaire.
3. Édition: charge `editingUser` → patch form → `update()` → sync accessCompetencies.
4. Suppression: confirmation → `delete(id)` → reload.

---
## 10. Gestion des Access Competencies
Processus actuel :
- Lecture existante via `userAccessCompetencyService.getByUserId()`
- Diff création / suppression calculée localement
- Exécutions en parallèle via `forkJoin()`

Amélioration : exposer un endpoint bulk (PUT /User/{id}/AccessCompetencies) pour réduire aller-retour.

---
## 11. Sécurité / Auth
- Token extrait du localStorage.
- Aucune vérification d’expiration → Risque 401 silencieux.
- Recommandé : intercepteur de rafraîchissement + redirection login sur 401.

---
## 12. Erreurs & Résilience
Actuel: message texte simple + console.error.  
Améliorer: 
- Centraliser handler (HttpErrorResponse → message friendly). 
- Ajouter loader / disable bouton sur envoi form.

---
## 13. Tests (suggestion de plan minimal)
Unit tests :
- `UserService` (mock HttpTestingController) : chaque méthode retourne la donnée attendue.
- `AdminUsersComponent` : 
  - Au démarrage: appelle `loadData()` → renseigne tableau.
  - Création: simuler succès → liste rechargée.
  - Synchronisation accessCompetencies: vérifier calcul diff (utiliser espions).

E2E (Playwright / Cypress) :
- Login (seed token).
- Créer utilisateur (vérifier présence ligne).
- Modifier (changement email ou rôle).
- Supprimer (ligne absente).

---
## 14. Décisions de conception clés
| Décision | Justification |
|----------|---------------|
| Fusion UserFullDto -> UserDto | Réduction duplication, simplifie templates |
| Champs UI optionnels | Minimiser friction compilation TypeScript strict |
| Standalone components | Aligné Angular 20, pas de NgModule inutile |
| Proxy `/api` | Permet environnement dev sans CORS complexe |

---
## 15. Améliorations futures
1. Remplacer `getFullUsers()` par `getAll()` si endpoint full absent (corriger bug parsing).  
2. Extraire logique AccessCompetencies en façade (pattern Facade / Store).  
3. Ajouter tri / pagination (MatTable + MatPaginator + MatSort).  
4. Snackbar unifié (service Notification) au lieu d’alert.  
5. Intercepteur refresh token + gestion 401.  
6. Mode recherche côté serveur (paramètres query).  
7. Internationalisation (i18n) labels (`$localize`).

---
## 16. Génération PDF (méthodes)
Option rapide (impression navigateur) :
1. Ouvrir ce fichier markdown rendu (extension VS Code Markdown Preview)  
2. Ctrl+P → Imprimer en PDF.

Option CLI (installer utilitaire) :
```bash
npm i --save-dev markdown-pdf
npx markdown-pdf docs/admin-users-crud.md -o docs/admin-users-crud.pdf
```
Alternative pandoc :
```bash
choco install pandoc -y
pandoc docs/admin-users-crud.md -o docs/admin-users-crud.pdf
```

---
## 17. Check rapide couverture actuelle
| Élément | Statut |
|---------|--------|
| CRUD liste | OK (lecture + suppression + édition + création) |
| Formulaire validation basique | OK |
| Sync accessCompetencies | OK mais perfectible |
| Endpoint full fonctionnel | A vérifier / corriger |
| Tests | À implémenter |
| UX feedback | Partiel |

---
## 18. Snippet remplacement route (si migration vers AdminUsersComponent)
```ts
// app.routes.ts (remplacer l’entrée existante 'admin/users')
{
  path: 'admin/users',
  loadComponent: () => import('./components/admin-users/admin-users.component').then(m => m.AdminUsersComponent),
  canActivate: [authGuard, RoleGuard],
  data: { roles: ['admin','administrateur'] },
  title: 'Gestion des utilisateurs - Admin'
}
```
Supprimer ensuite les routes `user-full-*` si non utilisées.

---
## 19. Convention payload Create / Update
- Create: `CreateUserDto` (sans id, avec champs obligatoires).
- Update: `UpdateUserDto` (contient `id`).  
Actuel: certains appels envoient `id:0` lors création → à aligner (retirer `id`).

---
## 20. Checklist migration finale
[ ] Vérifier endpoint `/User/full` dans Swagger / remplacer si absent  
[ ] Adapter `AdminUsersComponent` -> `getAll()`  
[ ] Modifier route `/admin/users`  
[ ] Retirer composants `user-full-*`  
[ ] Ajouter MatTable pagination  
[ ] Implémenter notifications snackbar  
[ ] Ajouter tests unitaires service + composant  
[ ] Générer PDF livrable  

---
Fin du document.
