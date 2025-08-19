TFE2025 – Application de gestion des utilisateurs full‑stack
Présentation
Ce dépôt contient le code de TFE2025, une application full‑stack réalisée dans le cadre d’un projet de fin d’études.
Le projet expose une API REST construite avec ASP.NET Core pour le back‑end et un front‑end autonome en Angular 20.
Le back‑end gère les utilisateurs, les rôles et les droits d’accès ;
le front‑end fournit une interface d’administration avec des graphiques et des tableaux de bord pour interagir avec l’API.

Le code est organisé en deux répertoires principaux :

Dossier	Description
Back	Solution C#/.NET contenant l’API Web, les services et les modèles d’entités
DiorFrontend	Application cliente Angular 20 autonome

Prérequis
Back‑end
SDK .NET 7.0 ou version ultérieure ;

Une instance SQL Server pour la base de données (la chaîne de connexion est définie dans Back/appsettings.json).

Front‑end
Node.js ≥ 18 avec npm ;

Angular CLI 20 installé globalement :npm install -g @angular/cli@20.

Mise en place
Cloner le dépôt :

bash
Copier
Modifier
git clone https://github.com/Toukist/TFE2025.git
cd TFE2025
Lancer le back‑end :

bash
Copier
Modifier
cd Back
# Restaurer les packages NuGet et compiler la solution
dotnet restore
dotnet build

# Mettre à jour appsettings.json avec votre chaîne de connexion et la clé secrète JWT
# Puis lancer l’API (par défaut sur https://localhost:5001)
dotnet run
L’API utilise l’authentification JWT et expose l’interface Swagger sur /swagger en environnement de développement.
Pensez à configurer Jwt:SecretKey, Jwt:Issuer et Jwt:Audience dans appsettings.json ou via des variables d’environnement.

Lancer le front‑end :

bash
Copier
Modifier
cd DiorFrontend
npm install
# Démarrer le serveur de développement Angular en utilisant la configuration de proxy vers l’API
npm run start
Par défaut, le front‑end est disponible sur http://localhost:4200 et redirige les appels API vers https://localhost:5001 via proxy.conf.json.
L’application est construite avec l’API autonome d’Angular 20 et n’utilise pas de NgModules.

Vérification du .gitignore
Le projet comprend un fichier .gitignore à la racine qui exclut les artefacts de compilation de Visual Studio (bin/, obj/, .vs/), les sorties Node/Angular (node_modules/, dist/, .angular/, etc.), les fichiers spécifiques aux systèmes d’exploitation et les paramètres d’IDE.
Un second .gitignore dans DiorFrontend couvre les artefacts spécifiques à Angular.
N’hésitez pas à ajouter d’autres répertoires (ex. coverage/ pour la couverture de tests) si nécessaire.

Contributions
Les contributions sont les bienvenues !
Pour proposer des modifications, merci de forker le dépôt et d’ouvrir une pull request.
Veillez à respecter la structure existante et à ajouter des tests unitaires pertinents.

Licence
Ce projet est fourni dans le cadre d’un travail de fin d’études et est destiné à des fins éducatives.
Sauf mention contraire, tous droits réservés.
