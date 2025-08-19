# TESTS DES NOUVEAUX ENDPOINTS - REFACTORING PHASE 1
# Utilisez ces commandes avec curl ou Postman

# 1. TEST STATUS API
curl -X GET "https://localhost:7201/api/Status" -H "accept: application/json"

# 2. TEST HEALTH CHECK
curl -X GET "https://localhost:7201/health" -H "accept: application/json"

# 3. TEST PROJETS (nécessite authentification JWT)
# D'abord obtenir un token via /api/auth/login puis :

curl -X GET "https://localhost:7201/api/Projet" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

curl -X POST "https://localhost:7201/api/Projet" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "nom": "Migration Cloud",
    "description": "Migration de l'\''infrastructure vers Azure",
    "teamId": 1,
    "dateDebut": "2025-02-01T00:00:00",
    "dateFin": "2025-06-30T00:00:00"
  }'

# 4. TEST ÉQUIPES
curl -X GET "https://localhost:7201/api/Team" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

curl -X GET "https://localhost:7201/api/Team/1/users" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# 5. TEST NOTIFICATIONS  
curl -X GET "https://localhost:7201/api/Notification/user/1" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

curl -X POST "https://localhost:7201/api/Notification" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "userId": 1,
    "type": "INFO", 
    "message": "Test notification depuis API refactorisée"
  }'

# 6. TEST TÂCHES
curl -X GET "https://localhost:7201/api/Task" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

curl -X GET "https://localhost:7201/api/Task/user/1" \
  -H "accept: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# 7. SWAGGER UI
# Ouvrir dans le navigateur: https://localhost:7201

echo "?? TOUS LES NOUVEAUX ENDPOINTS SONT PRÊTS POUR ANGULAR !"
echo "?? Documentation Swagger: https://localhost:7201" 
echo "?? Utilisez d'abord /api/auth/login pour obtenir un JWT token"