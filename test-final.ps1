# Test final de l'authentification

Write-Host "=====================================" -ForegroundColor Green
Write-Host "TEST FINAL AUTHENTIFICATION TFE-2025" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Test 1: Santé de l'API
Write-Host "`n1. Test de santé..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "✅ API en bonne santé: $($health.status)" -ForegroundColor Green
} catch {
    Write-Host "❌ API non accessible sur $baseUrl" -ForegroundColor Red
    exit 1
}

# Test 2: Endpoint de test auth
Write-Host "`n2. Test endpoint auth..." -ForegroundColor Yellow
try {
    $authTest = Invoke-RestMethod -Uri "$baseUrl/api/auth/test" -Method GET
    Write-Host "✅ Auth controller accessible" -ForegroundColor Green
} catch {
    Write-Host "❌ Auth controller non accessible: $_" -ForegroundColor Red
}

# Test 3: Liste des utilisateurs
Write-Host "`n3. Récupération des utilisateurs..." -ForegroundColor Yellow
try {
    $users = Invoke-RestMethod -Uri "$baseUrl/api/auth/users" -Method GET
    Write-Host "✅ $($users.Count) utilisateurs trouvés" -ForegroundColor Green
    $users | ForEach-Object {
        Write-Host "   - $($_.username): $($_.firstName) $($_.lastName)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Erreur: $_" -ForegroundColor Red
}

# Test 4: Connexion
Write-Host "`n4. Test de connexion..." -ForegroundColor Yellow
$loginBody = @{
    username = "manager01"
    password = "tfe2025"
    badgePhysicalNumber = ""
} | ConvertTo-Json

try {
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method POST `
        -Headers $headers `
        -Body $loginBody
    
    Write-Host "✅ CONNEXION RÉUSSIE!" -ForegroundColor Green
    Write-Host "   Token: $($response.token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "   User: $($response.user.userName)" -ForegroundColor Gray
} catch {
    Write-Host "❌ Échec de connexion: $_" -ForegroundColor Red
}

Write-Host "`n=====================================" -ForegroundColor Green
Write-Host "FIN DU TEST" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
