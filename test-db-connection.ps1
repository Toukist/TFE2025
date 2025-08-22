# Script de test de connexion DB
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST DE CONNEXION BASE DE DONNÉES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$connectionString = "Data Source=PC-CORENTIN\CLEAN22;Initial Catalog=Dior.Database;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"

try {
    # Test avec .NET SqlConnection
    Add-Type -AssemblyName System.Data
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    
    Write-Host "1. Tentative d'ouverture de connexion..." -ForegroundColor Yellow
    $connection.Open()
    Write-Host "✅ CONNEXION RÉUSSIE!" -ForegroundColor Green
    
    Write-Host "2. Test de la table USER..." -ForegroundColor Yellow
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT COUNT(*) FROM [USER]"
    $userCount = $command.ExecuteScalar()
    Write-Host "✅ Table USER existe avec $userCount utilisateurs" -ForegroundColor Green
    
    Write-Host "3. Vérification utilisateur admin01..." -ForegroundColor Yellow
    $command.CommandText = "SELECT Username, FirstName, LastName, Email, IsActive, passwordHash FROM [USER] WHERE Username = 'admin01'"
    $reader = $command.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "✅ Utilisateur admin01 trouvé:" -ForegroundColor Green
        Write-Host "   - Username: $($reader['Username'])" -ForegroundColor Gray
        Write-Host "   - Nom: $($reader['FirstName']) $($reader['LastName'])" -ForegroundColor Gray
        Write-Host "   - Email: $($reader['Email'])" -ForegroundColor Gray
        Write-Host "   - Actif: $($reader['IsActive'])" -ForegroundColor Gray
        Write-Host "   - Hash présent: $($reader['passwordHash'] -ne $null)" -ForegroundColor Gray
        
        if ($reader['passwordHash']) {
            $hash = $reader['passwordHash'].ToString()
            if ($hash.StartsWith('$2')) {
                Write-Host "   - Type: BCrypt hash" -ForegroundColor Green
            } else {
                Write-Host "   - Type: Texte clair ou autre" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "❌ Utilisateur admin01 non trouvé!" -ForegroundColor Red
    }
    
    $reader.Close()
    $connection.Close()
    
} catch {
    Write-Host "❌ ERREUR DE CONNEXION:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Vérifications suggérées:" -ForegroundColor Yellow
    Write-Host "1. SQL Server est-il démarré ?" -ForegroundColor Gray
    Write-Host "2. L'instance CLEAN22 existe-t-elle ?" -ForegroundColor Gray
    Write-Host "3. Permissions d'accès à la base ?" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
