-- Vérifier l'état des utilisateurs
SELECT 
    ID,
    Username,
    FirstName + ' ' + LastName as FullName,
    Email,
    IsActive,
    CASE 
        WHEN passwordHash IS NULL THEN 'NO PASSWORD'
        WHEN LEFT(passwordHash, 2) = '$2' THEN 'BCRYPT'
        ELSE 'PLAIN TEXT'
    END as PasswordType,
    LEFT(passwordHash, 30) as HashPreview
FROM [USER]
WHERE Username IN ('admin01', 'bruno.dupuis', 'marie.martin', 'jean.petit')
ORDER BY Username;

-- Si besoin, mettre à jour avec un hash BCrypt pour tfe2025
-- UPDATE [USER] 
-- SET passwordHash = '$2a$10$mYJP3XqvvPPyCCCCpCpyVOckXLqHyJxNV6rJj7V3s8jDGvEPxZq4e'
-- WHERE Username = 'admin01';
