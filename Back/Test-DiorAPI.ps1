# Script de test pour l'API Dior
# Exécutez ce script dans PowerShell pour tester les endpoints principaux

$baseUrl = "https://localhost:7135"  # Changez le port si nécessaire
$httpUrl = "http://localhost:5135"

Write-Host "?? Tests de l'API Dior" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

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

# Test 2: Login
Write-Host "`n?? Test 2: Authentication" -ForegroundColor Yellow
try {
    $loginBody = @{
        Username = "admin"
        Password = "admin"
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $loginResponse = Invoke-RestMethod -Uri "$httpUrl/api/auth/login" -Method POST -Body $loginBody -Headers $headers
    Write-Host "? Login réussi!" -ForegroundColor Green
    Write-Host "   Token: $($loginResponse.token.Substring(0,50))..." -ForegroundColor White
    Write-Host "   User: $($loginResponse.userName)" -ForegroundColor White
    
    # Sauvegarder le token pour les tests suivants
    $global:token = $loginResponse.token
}
catch {
    Write-Host "? Erreur Login: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Récupération des utilisateurs
Write-Host "`n?? Test 3: Liste des utilisateurs" -ForegroundColor Yellow
try {
    $users = Invoke-RestMethod -Uri "$httpUrl/api/user" -Method GET
    Write-Host "? Utilisateurs récupérés: $($users.Count)" -ForegroundColor Green
}
catch {
    Write-Host "? Erreur Users: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Récupération des équipes
Write-Host "`n?? Test 4: Liste des équipes" -ForegroundColor Yellow
try {
    $teams = Invoke-RestMethod -Uri "$httpUrl/api/team" -Method GET
    Write-Host "? Équipes récupérées: $($teams.Count)" -ForegroundColor Green
}
catch {
    Write-Host "? Erreur Teams: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Swagger documentation
Write-Host "`n?? Test 5: Documentation Swagger" -ForegroundColor Yellow
try {
    $swagger = Invoke-RestMethod -Uri "$httpUrl/swagger/v1/swagger.json" -Method GET
    Write-Host "? Swagger disponible!" -ForegroundColor Green
    Write-Host "   Endpoints disponibles: $($swagger.paths.PSObject.Properties.Count)" -ForegroundColor White
}
catch {
    Write-Host "? Erreur Swagger: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Validation du token (si login réussi)
if ($global:token) {
    Write-Host "`n?? Test 6: Validation du token" -ForegroundColor Yellow
    try {
        $authHeaders = @{
            "Authorization" = "Bearer $global:token"
        }
        
        $validation = Invoke-RestMethod -Uri "$httpUrl/api/auth/validate" -Method GET -Headers $authHeaders
        Write-Host "? Token valide!" -ForegroundColor Green
        Write-Host "   Validation: $($validation.valid)" -ForegroundColor White
    }
    catch {
        Write-Host "? Erreur Validation: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n?? Tests terminés!" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host "`n?? Accédez à Swagger UI:" -ForegroundColor Cyan
Write-Host "   HTTPS: https://localhost:7135/swagger" -ForegroundColor White
Write-Host "   HTTP:  http://localhost:5135/swagger" -ForegroundColor White