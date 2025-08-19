import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validateur personnalisé pour les numéros de téléphone belges
 * 
 * Formats acceptés :
 * - Mobiles : +32 4xx xxx xxx, 04xx xxx xxx, 04xx.xxx.xxx, 04xx-xxx-xxx
 * - Fixes : +32 [2-9]x xxx xxx, 0[2-9]x xxx xxx, 0[2-9]x.xxx.xxx, 0[2-9]x-xxx-xxx
 * - Avec ou sans espaces, points ou tirets
 * 
 * @returns ValidatorFn - Fonction de validation Angular
 */
export function belgianPhoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    
    // Si le champ est vide, laisser la validation required gérer cela
    if (!value) {
      return null;
    }
      // Nettoyer le numéro : supprimer espaces, points, tirets
    const cleanedValue = value.toString().replace(/[\s.-]/g, '');
    
    // Patterns pour les numéros belges
    const patterns = {
      // Format international mobile : +324xxxxxxxx
      internationalMobile: /^\+324[0-9]{8}$/,
      
      // Format international fixe : +32[2-9]xxxxxxxx  
      internationalFixed: /^\+32[2-9][0-9]{8}$/,
      
      // Format national mobile : 04xxxxxxxx
      nationalMobile: /^04[0-9]{8}$/,
      
      // Format national fixe : 0[2-9]xxxxxxxx
      nationalFixed: /^0[2-9][0-9]{8}$/
    };
    
    // Vérifier si le numéro correspond à un des patterns
    const isValid = Object.values(patterns).some(pattern => pattern.test(cleanedValue));
    
    if (!isValid) {
      return {
        belgianPhone: {
          message: 'Le numéro de téléphone doit être un numéro belge valide',
          actualValue: value,
          examples: [
            'Mobile: +32 478 123 456 ou 0478 123 456',
            'Fixe: +32 2 123 45 67 ou 02 123 45 67'
          ]
        }
      };
    }
    
    return null;
  };
}

/**
 * Formateur de numéro de téléphone belge
 * Formate automatiquement le numéro pendant la saisie
 * 
 * @param value - Valeur brute du numéro
 * @returns Numéro formaté
 */
export function formatBelgianPhone(value: string): string {
  if (!value) return '';
    // Nettoyer la valeur
  const cleaned = value.replace(/[\s.-]/g, '');
  
  // Format international
  if (cleaned.startsWith('+32')) {
    const number = cleaned.substring(3);
    if (number.length >= 9) {
      // Mobile +32 4xx xxx xxx
      if (number.startsWith('4')) {
        return `+32 ${number.substring(0, 3)} ${number.substring(3, 6)} ${number.substring(6, 9)}`;
      }
      // Fixe +32 [2-9]x xxx xxx
      else if (/^[2-9]/.test(number)) {
        return `+32 ${number.substring(0, 2)} ${number.substring(2, 5)} ${number.substring(5, 7)} ${number.substring(7, 9)}`;
      }
    }
  }
  
  // Format national
  if (cleaned.startsWith('0')) {
    // Mobile 0478 123 456
    if (cleaned.startsWith('04') && cleaned.length >= 10) {
      return `${cleaned.substring(0, 4)} ${cleaned.substring(4, 7)} ${cleaned.substring(7, 10)}`;
    }
    // Fixe 02 123 45 67
    else if (/^0[2-9]/.test(cleaned) && cleaned.length >= 9) {
      return `${cleaned.substring(0, 2)} ${cleaned.substring(2, 5)} ${cleaned.substring(5, 7)} ${cleaned.substring(7, 9)}`;
    }
  }
  
  return value;
}

/**
 * Validateur avec formatage automatique
 * Combine validation et formatage en temps réel
 */
export function belgianPhoneValidatorWithFormat(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const result = belgianPhoneValidator()(control);
    
    // Si valide, formater la valeur
    if (!result && control.value) {
      const formatted = formatBelgianPhone(control.value);
      if (formatted !== control.value) {
        // Mettre à jour la valeur formatée sans déclencher une nouvelle validation
        setTimeout(() => {
          control.setValue(formatted, { emitEvent: false });
        }, 0);
      }
    }
    
    return result;
  };
}
