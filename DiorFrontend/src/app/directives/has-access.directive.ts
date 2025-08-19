import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * 🔹 ÉTAPE 5 - Directive structurelle pour le contrôle d'accès
 * 
 * Utilisation dans les templates :
 * <div *appHasAccess="'PARTYKIT_ACCESS'">Contenu visible seulement si l'utilisateur a cette permission</div>
 * <button *appHasAccess="AccessCompetencyName.USER_MANAGEMENT">Gérer les utilisateurs</button>
 */
@Directive({
  selector: '[appHasAccess]',
  standalone: true
})
export class HasAccessDirective implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private hasAccess = false;

  @Input() set appHasAccess(permission: string) {
    this.checkAccess(permission);
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  ngOnInit() {
    // S'abonner aux changements d'AccessCompetencies
    this.authService.accessCompetencies$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        // Re-vérifier l'accès quand les permissions changent
        this.updateView();
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private checkAccess(permission: string) {
    this.hasAccess = this.authService.hasAccess(permission);
    this.updateView();
  }

  private updateView() {
    if (this.hasAccess) {
      // Afficher le contenu
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      // Masquer le contenu
      this.viewContainer.clear();
    }
  }
}
