import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ValidationError, ValidationResult } from '../../models/common.types';

/**
 * Service de validation centralisé
 * Fournit des validateurs personnalisés et des utilitaires de validation
 */
@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  // === VALIDATEURS SIMPLES ===

  /**
   * Validateur pour email avec domaines spécifiques autorisés
   */
  static emailWithDomains(allowedDomains: string[] = []): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      
      if (!emailRegex.test(control.value)) {
        return { email: { message: 'Format d\'email invalide' } };
      }

      if (allowedDomains.length > 0) {
        const domain = control.value.split('@')[1]?.toLowerCase();
        if (!allowedDomains.includes(domain)) {
          return { 
            emailDomain: { 
              message: `Domaine non autorisé. Domaines acceptés: ${allowedDomains.join(', ')}` 
            } 
          };
        }
      }

      return null;
    };
  }

  /**
   * Validateur pour mot de passe fort
   */
  static strongPassword(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const password = control.value;
      const errors: any = {};

      if (password.length < 8) {
        errors.minLength = { message: 'Le mot de passe doit contenir au moins 8 caractères' };
      }

      if (!/[A-Z]/.test(password)) {
        errors.uppercase = { message: 'Le mot de passe doit contenir au moins une majuscule' };
      }

      if (!/[a-z]/.test(password)) {
        errors.lowercase = { message: 'Le mot de passe doit contenir au moins une minuscule' };
      }

      if (!/\d/.test(password)) {
        errors.number = { message: 'Le mot de passe doit contenir au moins un chiffre' };
      }

      if (!/[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]/.test(password)) {
        errors.specialChar = { message: 'Le mot de passe doit contenir au moins un caractère spécial' };
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  /**
   * Validateur pour confirmation de mot de passe
   */
  static passwordMatch(passwordControlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.parent) return null;

      const passwordControl = control.parent.get(passwordControlName);
      if (!passwordControl) return null;

      if (control.value !== passwordControl.value) {
        return { passwordMatch: { message: 'Les mots de passe ne correspondent pas' } };
      }

      return null;
    };
  }

  /**
   * Validateur pour numéro de téléphone français
   */
  static frenchPhoneNumber(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const phoneRegex = /^(?:(?:\+|00)33|0)\s*[1-9](?:[\s.-]*\d{2}){4}$/;
      
      if (!phoneRegex.test(control.value)) {
        return { 
          frenchPhone: { 
            message: 'Numéro de téléphone français invalide (ex: 01 23 45 67 89)' 
          } 
        };
      }

      return null;
    };
  }

  /**
   * Validateur pour nom d'utilisateur
   */
  static username(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const username = control.value;
      const errors: any = {};

      if (username.length < 3) {
        errors.minLength = { message: 'Le nom d\'utilisateur doit contenir au moins 3 caractères' };
      }

      if (username.length > 50) {
        errors.maxLength = { message: 'Le nom d\'utilisateur ne peut pas dépasser 50 caractères' };
      }

      if (!/^[a-zA-Z0-9._-]+$/.test(username)) {
        errors.pattern = { 
          message: 'Le nom d\'utilisateur ne peut contenir que des lettres, chiffres, points, tirets et underscores' 
        };
      }

      if (/^[._-]/.test(username) || /[._-]$/.test(username)) {
        errors.invalidStart = { 
          message: 'Le nom d\'utilisateur ne peut pas commencer ou finir par un point, tiret ou underscore' 
        };
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  /**
   * Validateur pour date dans le futur
   */
  static futureDate(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const inputDate = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (inputDate <= today) {
        return { 
          futureDate: { 
            message: 'La date doit être dans le futur' 
          } 
        };
      }

      return null;
    };
  }

  /**
   * Validateur pour date dans le passé
   */
  static pastDate(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const inputDate = new Date(control.value);
      const today = new Date();
      today.setHours(23, 59, 59, 999);

      if (inputDate >= today) {
        return { 
          pastDate: { 
            message: 'La date doit être dans le passé' 
          } 
        };
      }

      return null;
    };
  }

  /**
   * Validateur pour âge minimum
   */
  static minimumAge(minAge: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const birthDate = new Date(control.value);
      const today = new Date();
      const age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();

      if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        const actualAge = age - 1;
        if (actualAge < minAge) {
          return { 
            minimumAge: { 
              message: `L'âge minimum requis est de ${minAge} ans` 
            } 
          };
        }
      } else if (age < minAge) {
        return { 
          minimumAge: { 
            message: `L'âge minimum requis est de ${minAge} ans` 
          } 
        };
      }

      return null;
    };
  }

  /**
   * Validateur pour URL
   */
  static url(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      try {
        new URL(control.value);
        return null;
      } catch {
        return { 
          url: { 
            message: 'URL invalide' 
          } 
        };
      }
    };
  }

  /**
   * Validateur pour fichier (taille et type)
   */
  static file(allowedTypes: string[] = [], maxSizeMB = 10): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const file = control.value as File;
      const errors: any = {};

      // Vérification du type
      if (allowedTypes.length > 0 && !allowedTypes.includes(file.type)) {
        errors.fileType = { 
          message: `Types de fichiers autorisés: ${allowedTypes.join(', ')}` 
        };
      }

      // Vérification de la taille
      const maxSizeBytes = maxSizeMB * 1024 * 1024;
      if (file.size > maxSizeBytes) {
        errors.fileSize = { 
          message: `La taille du fichier ne doit pas dépasser ${maxSizeMB} MB` 
        };
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  // === MÉTHODES UTILITAIRES ===

  /**
   * Convertit les erreurs Angular en format ValidationError
   */
  static formatValidationErrors(control: AbstractControl): ValidationError[] {
    if (!control.errors) return [];

    const errors: ValidationError[] = [];
    
    Object.keys(control.errors).forEach(key => {
      const error = control.errors![key];
      
      if (error && typeof error === 'object' && error.message) {
        errors.push({
          field: key,
          message: error.message,
          code: key
        });
      } else {
        // Messages par défaut pour les validateurs Angular standard
        const defaultMessages: Record<string, string> = {
          required: 'Ce champ est requis',
          email: 'Email invalide',
          min: `La valeur doit être supérieure ou égale à ${error?.min}`,
          max: `La valeur doit être inférieure ou égale à ${error?.max}`,
          minlength: `Au moins ${error?.requiredLength} caractères requis`,
          maxlength: `Maximum ${error?.requiredLength} caractères autorisés`,
          pattern: 'Format invalide'
        };

        errors.push({
          field: key,
          message: defaultMessages[key] || `Erreur de validation: ${key}`,
          code: key
        });
      }
    });

    return errors;
  }

  /**
   * Valide un objet complet et retourne le résultat
   */
  static validateObject(obj: any, validators: Record<string, ValidatorFn[]>): ValidationResult {
    const errors: ValidationError[] = [];

    Object.keys(validators).forEach(field => {
      const value = obj[field];
      const fieldValidators = validators[field];

      fieldValidators.forEach(validator => {
        const control = { value } as AbstractControl;
        const validationResult = validator(control);

        if (validationResult) {
          Object.keys(validationResult).forEach(errorKey => {
            const error = validationResult[errorKey];
            errors.push({
              field,
              message: error?.message || `Erreur ${errorKey} sur le champ ${field}`,
              code: errorKey
            });
          });
        }
      });
    });

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  /**
   * Validateur asynchrone générique
   */
  static asyncValidator<T>(
    validationFn: (value: T) => Observable<boolean>,
    errorMessage: string,
    debounceMs = 300
  ) {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) {
        return of(null);
      }

      return of(control.value).pipe(
        debounceTime(debounceMs),
        distinctUntilChanged(),
        switchMap(value => validationFn(value)),
        map(isValid => isValid ? null : { asyncValidation: { message: errorMessage } })
      );
    };
  }
}
