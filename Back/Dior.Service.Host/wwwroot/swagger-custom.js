// swagger-custom.js
// Améliorations pour l'interface Swagger UI de Dior Enterprise

window.addEventListener('DOMContentLoaded', function() {
    // Ajouter des instructions en français
    const observer = new MutationObserver(function(mutations) {
        const authSection = document.querySelector('.auth-wrapper');
        if (authSection && !document.querySelector('.custom-auth-instructions')) {
            addAuthInstructions();
        }
        
        // Améliorer l'affichage des modèles
        enhanceModelDisplay();
        
        // Ajouter des exemples pratiques
        addPracticalExamples();
    });
    
    observer.observe(document.body, { childList: true, subtree: true });
});

function addAuthInstructions() {
    const authSection = document.querySelector('.auth-wrapper');
    if (!authSection) return;
    
    const instructions = document.createElement('div');
    instructions.className = 'custom-auth-instructions';
    instructions.style.cssText = `
        background: #e3f2fd; 
        border: 1px solid #2196f3; 
        border-radius: 4px; 
        padding: 15px; 
        margin: 10px 0;
        font-family: sans-serif;
    `;
    
    instructions.innerHTML = `
        <h4 style="color: #1976d2; margin-top: 0;">?? Guide d'authentification Dior Enterprise</h4>
        <ol style="margin: 10px 0;">
            <li><strong>Connectez-vous</strong> avec l'endpoint <code>POST /api/Auth/login</code></li>
            <li><strong>Copiez le token</strong> retourné (sans les guillemets)</li>
            <li><strong>Cliquez sur Authorize</strong> ?? en haut de page</li>
            <li><strong>Collez votre token</strong> et cliquez Authorize</li>
        </ol>
        <div style="background: #fff3cd; border: 1px solid #ffeaa7; padding: 10px; border-radius: 4px; margin-top: 10px;">
            <strong>?? Astuce :</strong> Le token est valide 8 heures. Utilisez les endpoints selon votre rôle :
            <br>• <strong>Admin</strong> : Accès complet
            <br>• <strong>Manager</strong> : Équipes et projets  
            <br>• <strong>RH</strong> : Contrats et paies
            <br>• <strong>Opérateur</strong> : Consultation uniquement
        </div>
    `;
    
    authSection.parentNode.insertBefore(instructions, authSection);
}

function enhanceModelDisplay() {
    // Ajouter des descriptions aux modèles
    const models = document.querySelectorAll('.model-box');
    models.forEach(model => {
        const modelName = model.querySelector('.model-title')?.textContent;
        if (modelName && !model.querySelector('.custom-model-description')) {
            const description = getModelDescription(modelName);
            if (description) {
                const descDiv = document.createElement('div');
                descDiv.className = 'custom-model-description';
                descDiv.style.cssText = 'background: #f8f9fa; padding: 8px; margin: 5px 0; font-style: italic; border-left: 3px solid #28a745;';
                descDiv.textContent = description;
                model.querySelector('.model-title').parentNode.insertBefore(descDiv, model.querySelector('.model-title').nextSibling);
            }
        }
    });
}

function getModelDescription(modelName) {
    const descriptions = {
        'UserDto': '?? Représente un utilisateur avec ses rôles et informations personnelles',
        'ProjetDto': '?? Projet avec équipe assignée, dates et progression',
        'MessageDto': '?? Message interne entre utilisateurs ou équipes',
        'ContractDto': '?? Contrat employé avec type, salaire et durée',
        'PayslipDto': '?? Fiche de paie avec salaire brut, net et déductions',
        'TeamDto': '?? Équipe organisationnelle avec membres',
        'TaskDto': '? Tâche assignée avec statut et priorité',
        'NotificationDto': '?? Notification système ou utilisateur'
    };
    return descriptions[modelName];
}

function addPracticalExamples() {
    // Ajouter des exemples pratiques dans les endpoints
    const endpoints = document.querySelectorAll('.opblock-summary-description');
    endpoints.forEach(endpoint => {
        if (endpoint.textContent.includes('Login') && !endpoint.querySelector('.custom-example')) {
            const example = document.createElement('div');
            example.className = 'custom-example';
            example.style.cssText = 'background: #e8f5e8; padding: 8px; margin-top: 5px; border-radius: 3px; font-size: 12px;';
            example.innerHTML = `
                <strong>Exemple :</strong> 
                <code>{"username": "admin", "password": "admin123"}</code>
            `;
            endpoint.appendChild(example);
        }
    });
}

// Fonction utilitaire pour copier le token facilement
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        console.log('Token copié dans le presse-papiers');
    });
}

// Améliorer l'affichage des codes de statut HTTP
const originalFetch = window.fetch;
window.fetch = function(...args) {
    return originalFetch.apply(this, args).then(response => {
        // Log des appels API pour debug
        console.log(`?? ${args[1]?.method || 'GET'} ${args[0]} ? ${response.status}`);
        return response;
    });
};