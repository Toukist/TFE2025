import { HttpInterceptorFn } from '@angular/common/http';

/**
 * Intercepteur HTTP fonctionnel pour l'authentification JWT
 * 
 * Ajoute automatiquement le token JWT dans l'en-tête Authorization
 * de toutes les requêtes HTTP sortantes si un token est présent
 * dans le localStorage.
 * 
 * Compatible avec le Server-Side Rendering (SSR).
 * 
 * @param req - La requête HTTP
 * @param next - Le gestionnaire suivant dans la chaîne d'intercepteurs
 * @returns Observable de la réponse HTTP
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Ne pas ajouter le token sur la route de login
  if (req.url.includes('/auth/login')) {
    return next(req);
  }

  if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
    const token = localStorage.getItem('authToken');
    if (token) {
      req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    }
  }
  return next(req);
};
