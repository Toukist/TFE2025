import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, isDevMode } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';

/**
 * Intercepteur d'erreurs HTTP global.
 * - 401: déconnexion et redirection vers /login (avec returnUrl si présent)
 * - 403: SnackBar + redirection vers /unauthorized
 * - 404: SnackBar discrète
 * - 0 ou autres: SnackBar générique
 */
export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);
  const router = inject(Router);
  const auth = inject(AuthService);

  const isBrowser = typeof window !== 'undefined';

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const url = router.url || '/';

      if (error.status === 401) {
        // Session expirée / non autorisé -> cleanup et redirection login
        if (isBrowser) snackBar.open('Session expirée, veuillez vous reconnecter.', 'Fermer', { duration: 4000 });
        try { auth.logout(); } catch (e) { /* ignore */ }
        router.navigate(['/login'], { queryParams: { returnUrl: url } });
      } else if (error.status === 403) {
        if (isBrowser) snackBar.open("Accès refusé: droits insuffisants.", 'Fermer', { duration: 4000 });
        router.navigate(['/unauthorized']);
      } else if (error.status === 404) {
        // 404: afficher un message discret, éviter le spam en dev
        if (isBrowser) snackBar.open('Ressource introuvable.', 'Fermer', { duration: 3000 });
      } else if (error.status === 0) {
        if (isBrowser) snackBar.open("Impossible de joindre le serveur.", 'Fermer', { duration: 4000 });
      } else {
        const msg = error.error?.message || error.message || 'Une erreur est survenue.';
        if (isBrowser) snackBar.open(msg, 'Fermer', { duration: 4000 });
      }

      if (isDevMode()) {
        // Log détaillé uniquement en dev
        console.error('[httpErrorInterceptor]', req.method, req.url, error);
      }

      return throwError(() => error);
    })
  );
};
