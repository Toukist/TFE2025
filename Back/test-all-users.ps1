# Test complet des utilisateurs
Write-Host "🚀 DÉMARRAGE DU TEST COMPLET DES UTILISATEURS" -ForegroundColor Green
Write-Host ""

# 1. Tester la connexion à la base de données
Write-Host "1. Test de connexion DB..." -ForegroundColor Yellow
$connectionString = "Server=PC-CORENTIN\CLEAN22;Database=Dior.Database;Integrated Security=true;TrustServerCertificate=true;"
try {
    $sqlConn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $sqlConn.Open()
    
    $cmd = New-Object System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [USER] WHERE IsActive = 1", $sqlConn)
    $activeUsers = $cmd.ExecuteScalar()
    Write-Host "✅ DB connectée, $activeUsers utilisateurs actifs" -ForegroundColor Green
    $sqlConn.Close()
} catch {
    Write-Host "❌ Erreur DB: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# 2. Lancer le serveur et attendre qu'il soit prêt
Write-Host ""
Write-Host "2. Lancement du serveur..." -ForegroundColor Yellow
Set-Location "C:\Users\Corentin\Desktop\TFE-2025\Back\Dior.Service.Host"

$serverProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -NoNewWindow -RedirectStandardOutput "server_output.log" -RedirectStandardError "server_error.log"

# Attendre que le serveur démarre
Write-Host "Attente du démarrage du serveur..."
$timeout = 30
$elapsed = 0
$serverReady = $false

while ($elapsed -lt $timeout -and !$serverReady) {
    Start-Sleep -Seconds 1
    $elapsed++
    try {
        $testResponse = Invoke-WebRequest -Uri "http://localhost:5000" -Method GET -TimeoutSec 5 -ErrorAction Stop
        $serverReady = $true
        Write-Host "✅ Serveur démarré sur http://localhost:5000" -ForegroundColor Green
    } catch {
        Write-Host "." -NoNewline
    }
}

if (!$serverReady) {
    Write-Host ""
    Write-Host "❌ Timeout - Le serveur n'a pas démarré dans les $timeout secondes" -ForegroundColor Red
    $serverProcess.Kill()
    exit
}

Write-Host ""

# 3. Tester les différents utilisateurs
$usersToTest = @("admin01", "manager01", "rh01", "operateur01", "multi01")

foreach ($user in $usersToTest) {
    Write-Host "3. Test $user..." -ForegroundColor Yellow -NoNewline
    
    try {
        $body = @{
            username = $user
            password = "tfe2025"
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -ContentType "application/json" -Body $body -TimeoutSec 10
        
        if ($response.token) {
            Write-Host " ✅ SUCCÈS - Token reçu" -ForegroundColor Green
            Write-Host "   Utilisateur: $($response.userName), ID: $($response.userId)"
        } else {
            Write-Host " ❌ Échec - Pas de token" -ForegroundColor Red
        }
    } catch {
        Write-Host " ❌ Erreur: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# 4. Arrêter le serveur
Write-Host ""
Write-Host "4. Arrêt du serveur..." -ForegroundColor Yellow
$serverProcess.Kill()
Write-Host "✅ Serveur arrêté" -ForegroundColor Green

Write-Host ""
Write-Host "🏁 TEST TERMINÉ" -ForegroundColor Green
