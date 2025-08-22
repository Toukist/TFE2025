# Script simple de vérification des utilisateurs
$connectionString = "Server=PC-CORENTIN\CLEAN22;Database=Dior.Database;Integrated Security=true;TrustServerCertificate=true;"

try {
    $conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $conn.Open()
    
    # Compter les utilisateurs
    $cmd = New-Object System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM [USER]", $conn)
    $total = $cmd.ExecuteScalar()
    Write-Host "=== BASE DE DONNÉES DIOR ==="
    Write-Host "Total utilisateurs: $total"
    Write-Host ""
    
    # Lister tous les utilisateurs
    $cmd.CommandText = "SELECT ID, Username, FirstName, LastName, IsActive, passwordHash FROM [USER] ORDER BY Username"
    $reader = $cmd.ExecuteReader()
    
    Write-Host "LISTE COMPLÈTE DES UTILISATEURS:"
    Write-Host "---------------------------------"
    
    while ($reader.Read()) {
        $id = $reader["ID"]
        $username = $reader["Username"]
        $firstName = $reader["FirstName"]
        $lastName = $reader["LastName"] 
        $isActive = $reader["IsActive"]
        $hash = $reader["passwordHash"].ToString()
        
        $hashType = if ($hash.StartsWith('$2')) { "BCrypt" } else { "Plain" }
        $status = if ($isActive) { "ACTIF" } else { "INACTIF" }
        
        Write-Host "$id | $username | $firstName $lastName | $status | $hashType"
    }
    
    $reader.Close()
    $conn.Close()
    Write-Host ""
    Write-Host "✅ Connexion DB réussie - Tous les utilisateurs sont visibles"
    
} catch {
    Write-Host "❌ Erreur DB: $($_.Exception.Message)"
}
