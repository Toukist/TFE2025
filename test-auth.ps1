# test-auth.ps1
# Script de diagnostic pour le problème d'authentification

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST D'AUTHENTIFICATION DIOR" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$baseUrl = "http://localhost:5000"
$testUsername = "admin01"
$testPassword = "tfe2025"

# Désactiver la vérification SSL pour les tests locaux
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
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Fonction pour tester une URL
function Test-Endpoint {
    param($Url, $Method = "GET", $Body = $null)
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
            "Accept" = "application/json"
        }
        
        if ($Method -eq "GET") {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers -ErrorAction Stop
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers -Body $Body -ErrorAction Stop
        }
        return $response
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 401) {
            Write-Host "❌ 401 Unauthorized" -ForegroundColor Red
        } else {
            Write-Host "❌ Erreur: $_" -ForegroundColor Red
        }
        return $null
    }
}

# Test 1: Vérifier que l'API est accessible
Write-Host "1. Test de connexion à l'API..." -ForegroundColor Yellow
try {
    $swaggerUrl = "$baseUrl/swagger/index.html"
    $webResponse = Invoke-WebRequest -Uri $swaggerUrl -UseBasicParsing -ErrorAction SilentlyContinue
    if ($webResponse.StatusCode -eq 200) {
        Write-Host "✅ API accessible sur $baseUrl" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️ L'API n'est pas accessible sur $baseUrl" -ForegroundColor Red
    Write-Host "Essayez avec le port 7201..." -ForegroundColor Yellow
    $baseUrl = "https://localhost:7201"
}

# Test 2: Tester l'endpoint de diagnostic (s'il existe)
Write-Host ""
Write-Host "2. Test de l'endpoint de diagnostic..." -ForegroundColor Yellow
$testUrl = "$baseUrl/api/auth/test/$testUsername"
$testResult = Test-Endpoint -Url $testUrl

if ($testResult) {
    Write-Host "✅ Utilisateur trouvé:" -ForegroundColor Green
    Write-Host "   - Username: $($testResult.username)"
    Write-Host "   - Actif: $($testResult.isActive)"
    Write-Host "   - Type hash: $($testResult.hashType)"
    Write-Host "   - Test password: $($testResult.passwordTestResult)"
}

# Test 3: Tenter la connexion
Write-Host ""
Write-Host "3. Test de connexion avec admin01..." -ForegroundColor Yellow

$loginBody = @{
    username = $testUsername
    password = $testPassword
    badgePhysicalNumber = ""
} | ConvertTo-Json

$loginUrl = "$baseUrl/api/auth/login"
Write-Host "   URL: $loginUrl" -ForegroundColor Gray
Write-Host "   Body: $loginBody" -ForegroundColor Gray

$loginResult = Test-Endpoint -Url $loginUrl -Method "POST" -Body $loginBody

if ($loginResult) {
    Write-Host "✅ CONNEXION RÉUSSIE!" -ForegroundColor Green
    Write-Host "   Token: $($loginResult.token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "   User ID: $($loginResult.user.id)" -ForegroundColor Gray
} else {
    Write-Host ""
    Write-Host "🔧 TENTATIVE DE CORRECTION..." -ForegroundColor Yellow
    
    # Essayer de corriger le mot de passe
    $fixUrl = "$baseUrl/api/auth/fix-password"
    $fixBody = @{
        username = $testUsername
        password = $testPassword
    } | ConvertTo-Json
    
    $fixResult = Test-Endpoint -Url $fixUrl -Method "POST" -Body $fixBody
    
    if ($fixResult) {
        Write-Host "✅ Mot de passe corrigé, nouvelle tentative..." -ForegroundColor Green
        Start-Sleep -Seconds 1
        
        # Réessayer la connexion
        $loginResult2 = Test-Endpoint -Url $loginUrl -Method "POST" -Body $loginBody
        if ($loginResult2) {
            Write-Host "✅ CONNEXION RÉUSSIE APRÈS CORRECTION!" -ForegroundColor Green
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FIN DU TEST" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
