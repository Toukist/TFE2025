# Script de test pour l'API Dior - SYSTÈME D'AUTHENTIFICATION COMPLET
# Exécutez ce script dans PowerShell pour tester les endpoints d'authentification

$baseUrl = "https://localhost:7135"  # Changez le port si nécessaire
$httpUrl = "http://localhost:5135"

Write-Host "?? Tests du Système d'Authentification Dior" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green

# Test 1: Status de l'API
Write-Host "`n?? Test 1: Status de l'API" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$httpUrl/api/status" -Method GET
    Write-Host "? Status API: $($response.status)" -ForegroundColor Green
    Write-Host "   Message: $($response.message)" -ForegroundColor White
}
catch {
    Write-Host "? Erreur Status: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Login par Username/Password
Write-Host "`n?? Test 2: Authentication par Username/Password" -ForegroundColor Yellow
try {
    $loginBody = @{
        Username = "admin"
        Password = "admin"
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $loginResponse = Invoke-RestMethod -Uri "$httpUrl/api/auth/login" -Method POST -Body $loginBody -Headers $headers
    Write-Host "? Login réussi par credentials!" -ForegroundColor Green
    Write-Host "   User ID: $($loginResponse.userId)" -ForegroundColor White
    Write-Host "   Username: $($loginResponse.userName)" -ForegroundColor White
    Write-Host "   Token: $($loginResponse.token.Substring(0,50))..." -ForegroundColor White
    Write-Host "   Rôles: $($loginResponse.roles -join ', ')" -ForegroundColor White
    Write-Host "   Compétences: $($loginResponse.accessCompetencies -join ', ')" -ForegroundColor White
    
    # Sauvegarder le token pour les tests suivants
    $global:token = $loginResponse.token
    $global:userId = $loginResponse.userId
}
catch {
    Write-Host "? Erreur Login Credentials: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Login par Badge (test mockée)
Write-Host "`n??? Test 3: Authentication par Badge Physique" -ForegroundColor Yellow
try {
    $badgeLoginBody = @{
        BadgePhysicalNumber = "12345"
    } | ConvertTo-Json

    $badgeResponse = Invoke-RestMethod -Uri "$httpUrl/api/auth/login" -Method POST -Body $badgeLoginBody -Headers $headers
    Write-Host "? Login réussi par badge!" -ForegroundColor Green
    Write-Host "   Badge: $($badgeResponse.badgePhysicalNumber)" -ForegroundColor White
}
catch {
    Write-Host "??  Badge non trouvé (normal en test): $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test 4: Validation du token
if ($global:token) {
    Write-Host "`n?? Test 4: Validation du token JWT" -ForegroundColor Yellow
    try {
        $authHeaders = @{
            "Authorization" = "Bearer $global:token"
        }
        
        $validation = Invoke-RestMethod -Uri "$httpUrl/api/auth/validate" -Method GET -Headers $authHeaders
        Write-Host "? Token valide!" -ForegroundColor Green
        Write-Host "   Validation: $($validation.valid)" -ForegroundColor White
        Write-Host "   User ID: $($validation.userId)" -ForegroundColor White
        Write-Host "   Rôles: $($validation.roles -join ', ')" -ForegroundColor White
    }
    catch {
        Write-Host "? Erreur Validation: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 5: Récupération du profil utilisateur connecté
if ($global:token) {
    Write-Host "`n?? Test 5: Profil utilisateur connecté (/me)" -ForegroundColor Yellow
    try {
        $authHeaders = @{
            "Authorization" = "Bearer $global:token"
        }
        
        $userProfile = Invoke-RestMethod -Uri "$httpUrl/api/auth/me" -Method GET -Headers $authHeaders
        Write-Host "? Profil récupéré!" -ForegroundColor Green
        Write-Host "   ID: $($userProfile.id)" -ForegroundColor White
        Write-Host "   Username: $($userProfile.username)" -ForegroundColor White
        Write-Host "   Nom complet: $($userProfile.firstName) $($userProfile.lastName)" -ForegroundColor White
        Write-Host "   Email: $($userProfile.email)" -ForegroundColor White
        Write-Host "   Équipe: $($userProfile.teamName)" -ForegroundColor White
        Write-Host "   Rôles: $($userProfile.roles.name -join ', ')" -ForegroundColor White
    }
    catch {
        Write-Host "? Erreur Profil: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 6: Test de déconnexion
if ($global:token) {
    Write-Host "`n?? Test 6: Déconnexion" -ForegroundColor Yellow
    try {
        $authHeaders = @{
            "Authorization" = "Bearer $global:token"
        }
        
        $logout = Invoke-RestMethod -Uri "$httpUrl/api/auth/logout" -Method POST -Headers $authHeaders
        Write-Host "? Déconnexion réussie!" -ForegroundColor Green
        Write-Host "   Message: $($logout.message)" -ForegroundColor White
    }
    catch {
        Write-Host "? Erreur Logout: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 7: Documentation Swagger
Write-Host "`n?? Test 7: Documentation Swagger" -ForegroundColor Yellow
try {
    $swagger = Invoke-RestMethod -Uri "$httpUrl/swagger/v1/swagger.json" -Method GET
    Write-Host "? Swagger disponible!" -ForegroundColor Green
    Write-Host "   Endpoints disponibles: $($swagger.paths.PSObject.Properties.Count)" -ForegroundColor White
    
    # Compter les endpoints d'authentification
    $authEndpoints = $swagger.paths.PSObject.Properties | Where-Object { $_.Name -like "*auth*" }
    Write-Host "   Endpoints auth: $($authEndpoints.Count)" -ForegroundColor White
}
catch {
    Write-Host "? Erreur Swagger: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n?? Tests d'authentification terminés!" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Green
Write-Host "`n?? Accédez à Swagger UI pour tester manuellement:" -ForegroundColor Cyan
Write-Host "   HTTPS: https://localhost:7135/swagger" -ForegroundColor White
Write-Host "   HTTP:  http://localhost:5135/swagger" -ForegroundColor White
Write-Host "`n?? Endpoints d'authentification disponibles:" -ForegroundColor Cyan
Write-Host "   POST /api/auth/login (badge OU username/password)" -ForegroundColor White
Write-Host "   GET  /api/auth/validate (avec Authorization header)" -ForegroundColor White
Write-Host "   GET  /api/auth/me (profil utilisateur connecté)" -ForegroundColor White
Write-Host "   POST /api/auth/logout (déconnexion)" -ForegroundColor White
Write-Host "   POST /api/auth/change-password (changer mot de passe)" -ForegroundColor White

Write-Host "`n?? Exemple d'utilisation Frontend Angular:" -ForegroundColor Cyan
Write-Host "   1. Login : POST /api/auth/login avec { username: 'admin', password: 'admin' }" -ForegroundColor White
Write-Host "   2. Récupérer le token dans la réponse" -ForegroundColor White  
Write-Host "   3. Ajouter 'Authorization: Bearer <token>' aux requêtes suivantes" -ForegroundColor White
Write-Host "   4. Utiliser /api/auth/me pour récupérer le profil complet" -ForegroundColor White