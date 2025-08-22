# test-backend-final.ps1
# Test complet du backend TFE-2025

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "     TEST COMPLET BACKEND TFE-2025        " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5100"

# Ignorer les erreurs SSL
add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) {
        return true;
    }
}
"@ -ErrorAction SilentlyContinue
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# TEST 1: Santé API
Write-Host "1️⃣ Test de santé de l'API..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "✅ API en ligne: $($health.status)" -ForegroundColor Green
    Write-Host "   Environnement: $($health.environment)" -ForegroundColor Gray
} catch {
    Write-Host "❌ API non accessible sur $baseUrl" -ForegroundColor Red
    Write-Host "   Erreur: $_" -ForegroundColor Red
    exit 1
}

# TEST 2: Santé Auth Controller
Write-Host "`n2️⃣ Test Auth Controller..." -ForegroundColor Yellow
try {
    $authHealth = Invoke-RestMethod -Uri "$baseUrl/api/auth/health" -Method GET
    Write-Host "✅ Auth Controller opérationnel" -ForegroundColor Green
    Write-Host "   Database: $($authHealth.database)" -ForegroundColor Gray
} catch {
    Write-Host "❌ Auth Controller non accessible: $_" -ForegroundColor Red
}

# TEST 3: Liste utilisateurs
Write-Host "`n3️⃣ Récupération des utilisateurs..." -ForegroundColor Yellow
try {
    $users = Invoke-RestMethod -Uri "$baseUrl/api/auth/users" -Method GET
    Write-Host "✅ $($users.Count) utilisateurs trouvés:" -ForegroundColor Green
    $users | Select-Object -First 5 | ForEach-Object {
        $status = if ($_.isActive) { "✓" } else { "✗" }
        Write-Host "   $status $($_.username) - $($_.name) [$($_.passwordType)]" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Erreur: $_" -ForegroundColor Red
}

# TEST 4: Test de connexion
Write-Host "`n4️⃣ Test de connexion avec manager01..." -ForegroundColor Yellow

$loginTests = @(
    @{ username = "manager01"; password = "tfe2025" },
    @{ username = "rh01"; password = "tfe2025" },
    @{ username = "operateur01"; password = "tfe2025" }
)

foreach ($test in $loginTests) {
    Write-Host "`n   Testing: $($test.username)" -ForegroundColor Cyan
    
    $body = $test | ConvertTo-Json
    $headers = @{ "Content-Type" = "application/json" }
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
            -Method POST `
            -Headers $headers `
            -Body $body
        
        if ($response.token) {
            Write-Host "   ✅ Connexion réussie!" -ForegroundColor Green
            Write-Host "      Token: $($response.token.Substring(0, 50))..." -ForegroundColor Gray
            Write-Host "      User ID: $($response.user.id)" -ForegroundColor Gray
            Write-Host "      Roles: $($response.user.roles.Count) rôle(s)" -ForegroundColor Gray
        }
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 401) {
            Write-Host "   ⚠️ Authentification échouée (401)" -ForegroundColor Yellow
        } else {
            Write-Host "   ❌ Erreur $statusCode : $_" -ForegroundColor Red
        }
    }
}

# TEST 5: Swagger
Write-Host "`n5️⃣ Test Swagger..." -ForegroundColor Yellow
try {
    $swagger = Invoke-WebRequest -Uri "$baseUrl/swagger/index.html" -UseBasicParsing
    if ($swagger.StatusCode -eq 200) {
        Write-Host "✅ Swagger accessible sur $baseUrl/swagger" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️ Swagger non accessible" -ForegroundColor Yellow
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "           TEST TERMINÉ                    " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs importantes:" -ForegroundColor White
Write-Host "  API:     $baseUrl" -ForegroundColor Gray
Write-Host "  Swagger: $baseUrl/swagger" -ForegroundColor Gray
Write-Host "  Health:  $baseUrl/health" -ForegroundColor Gray
Write-Host ""
