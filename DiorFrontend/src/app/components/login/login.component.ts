import { Component, OnInit, OnDestroy, ChangeDetectorRef, AfterViewInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { takeUntil, filter, tap, first } from 'rxjs/operators'; // first ajouté
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy, AfterViewInit {
  loginForm!: FormGroup;
  badgeForm!: FormGroup;
  loginType: 'credentials' | 'badge' = 'badge';  errorMessage = '';
  returnUrl = '';
  isLoading$: Observable<boolean>;
  showPassword = false;
  badgeBuffer = '';
  private badgeTimeout: any = null;
  private readonly BADGE_INPUT_TIMEOUT = 300;
  private destroy$ = new Subject<void>();

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly fb: FormBuilder,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.isLoading$ = this.authService.isLoading$;
    this.initializeForms();
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';

    this.authService.isLoggedIn$.pipe(
      takeUntil(this.destroy$),
      filter(isLoggedIn => isLoggedIn === true),
      tap(() => this.handleRedirect())
    ).subscribe();
  }

  ngAfterViewInit(): void {
    if (this.loginType === 'badge') {
      this.setupBadgeScanFocus();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.badgeTimeout) {
      clearTimeout(this.badgeTimeout);
    }
    this.cleanupBodyListeners();
  }

  private initializeForms(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(2)]],
      password: ['', [Validators.required, Validators.minLength(3)]]
    });

    this.badgeForm = this.fb.group({
      badgeNumber: ['', [Validators.required, Validators.pattern(/^[0-9]+$/)]]
    });
  }

  setLoginType(type: 'credentials' | 'badge'): void {
    this.loginType = type;
    this.clearForm();
    if (type === 'badge') {
      this.setupBadgeScanFocus();
    } else {
      this.cleanupBodyListeners();
    }
  }

  clearForm(): void {
    this.loginForm.reset();
    this.badgeForm.reset();
    this.badgeBuffer = '';
    this.errorMessage = '';
  }

  onSubmit(): void {
    console.log('submit', this.loginType, this.loginForm.value, this.badgeForm.value);
    this.authService.isLoading$.pipe(first()).subscribe(loading => {
      if (loading) return;
      this.errorMessage = '';

      if (this.loginType === 'credentials') {
        if (this.loginForm.valid) {
          this.loginWithCredentials();
        } else {
          this.errorMessage = 'Veuillez remplir correctement tous les champs.';
          this.markFormGroupTouched(this.loginForm);
        }
      } else {
        if (this.badgeForm.valid && this.badgeForm.value.badgeNumber) {
          this.triggerBadgeLogin(this.badgeForm.value.badgeNumber);
        } else if (this.badgeBuffer) {
          this.triggerBadgeLogin(this.badgeBuffer);
        } else {
          this.errorMessage = 'Veuillez scanner votre badge ou saisir un numéro de badge valide.';
          this.markFormGroupTouched(this.badgeForm);
        }
      }
    });
  }

  private loginWithCredentials(): void {
    const { username, password } = this.loginForm.value;
    this.authService.loginWithCredentials(username.trim(), password)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          console.log('Réponse login:', response);
          // Si le service ne redirige pas, on force la redirection ici
          if (response?.success && response?.token && response?.user && response?.userId) {
            // Redirection déjà gérée par le service, mais on peut forcer si besoin :
            // this.authService.performRoleBasedRedirection();
          } else {
            this.errorMessage = 'Réponse inattendue du serveur.';
          }
        },
        error: (error) => {
          this.errorMessage = this.getErrorMessage(error);
          this.cdr.detectChanges();
        }
      });
  }

  private triggerBadgeLogin(badgeNumber: string): void {
    this.authService.isLoading$.pipe(first()).subscribe(loading => {
      if (!badgeNumber || loading) return;

      this.errorMessage = '';
      this.authService.loginWithBadge(badgeNumber.trim())
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          error: (error) => {
            this.errorMessage = this.getErrorMessage(error);
            this.badgeForm.get('badgeNumber')?.setValue('');
            this.badgeBuffer = '';
            this.setBodyBadgeStatus('error');
            this.cdr.detectChanges();
          }
        });
    });
  }

  private getErrorMessage(error: any): string {
    if (error?.status === 401) return '🔒 Identifiants ou badge incorrects.';
    if (error?.status === 404) return '👤 Utilisateur non trouvé.';
    if (error?.status === 403) return '🚫 Accès refusé ou compte inactif.';
    if (error?.status === 0 || error?.name === 'TimeoutError') return '🌐 Problème de connexion au serveur. Veuillez réessayer.';
    return error?.error?.message || error?.message || 'Une erreur inconnue est survenue.';
  }

  private handleRedirect(): void {
    if (this.returnUrl && this.returnUrl !== '/login' && this.returnUrl !== '/') {
      this.router.navigateByUrl(this.returnUrl);
    } else {
      this.authService.performRoleBasedRedirection();
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      control.updateValueAndValidity();
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  get username() { return this.loginForm.get('username'); }
  get password() { return this.loginForm.get('password'); }
  get badgeNumber() { return this.badgeForm.get('badgeNumber'); }

  getFieldError(field: string, form: FormGroup): string {
    const control = form.get(field);
    if (!control || !control.errors) return '';
    if (control.errors['required']) return 'Ce champ est requis.';
    if (control.errors['minlength']) return `Minimum ${control.errors['minlength'].requiredLength} caractères.`;
    if (control.errors['pattern']) return 'Format invalide.';
    return 'Champ invalide.';
  }

  @HostListener('window:keypress', ['$event'])
  handleKeyPress(event: KeyboardEvent): void {
    if (this.loginType !== 'badge' || (event.target instanceof HTMLInputElement && event.target.type !== 'hidden')) {
      return;
    }

    if (event.key === 'Enter') {
      if (this.badgeBuffer.length > 0) {
        event.preventDefault();
        console.log('Badge Enter detected, buffer:', this.badgeBuffer);
        this.setBodyBadgeStatus('scanned');
        this.badgeForm.get('badgeNumber')?.setValue(this.badgeBuffer);
        this.triggerBadgeLogin(this.badgeBuffer);
        this.badgeBuffer = '';
        clearTimeout(this.badgeTimeout);
      }
    } else {
      if (event.key && /^[a-zA-Z0-9]$/.test(event.key)) {
         this.badgeBuffer += event.key;
      }
     
      clearTimeout(this.badgeTimeout);
      this.badgeTimeout = setTimeout(() => {
        if (this.badgeBuffer.length > 0) {
          console.log('Badge timeout, buffer:', this.badgeBuffer);
          this.setBodyBadgeStatus('scanned');
          this.badgeForm.get('badgeNumber')?.setValue(this.badgeBuffer);
          this.triggerBadgeLogin(this.badgeBuffer);
          this.badgeBuffer = '';
        }
      }, this.BADGE_INPUT_TIMEOUT);
    }
  }

  private setupBadgeScanFocus(): void {
    const body = document.querySelector('body');
    if (body) {
      body.setAttribute('tabindex', '-1');
      body.style.outline = 'none';
      this.setBodyBadgeStatus('listening');
      console.log('Badge scan mode enabled. Listening for badge input.');
    }
  }

  private cleanupBodyListeners(): void {
    const body = document.querySelector('body');
    if (body) {
      body.removeAttribute('tabindex');
      body.style.outline = '';
      this.setBodyBadgeStatus(null);
    }
  }

  private setBodyBadgeStatus(status: 'listening' | 'scanned' | 'error' | null): void {
    const body = document.querySelector('body');
    if (body) {
      body.classList.remove('badge-listening', 'badge-scanned', 'badge-error');
      if (status) {
        body.classList.add(`badge-${status}`);
      }
    }
  }
}
