import { Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';
import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import 'jspdf-autotable';

@Injectable({
  providedIn: 'root'
})
export class DataTableUtilsService {
  private datePipe = new DatePipe('fr-FR');  /**
   * Formatters prédéfinis pour les colonnes courantes
   */
  readonly formatters = {
    // Format pour les dates
    date: (value: unknown) => {
      if (!value) return '-';
      const date = typeof value === 'string' ? new Date(value) : 
                   value instanceof Date ? value : new Date(String(value));
      return this.datePipe.transform(date, 'dd/MM/yyyy') || '-';
    },

    // Format pour les dates avec heure
    datetime: (value: unknown) => {
      if (!value) return '-';
      const date = typeof value === 'string' ? new Date(value) : 
                   value instanceof Date ? value : new Date(String(value));
      return this.datePipe.transform(date, 'dd/MM/yyyy HH:mm') || '-';
    },

    // Format pour les booléens (Oui/Non)
    yesNo: (value: boolean) => value ? 'Oui' : 'Non',

    // Format pour les booléens avec badges colorés
    activeStatus: (value: boolean) => {
      return value 
        ? '<span class="badge badge-success">Actif</span>'
        : '<span class="badge badge-danger">Inactif</span>';
    },    // Format pour les emails
    email: (value: string) => {
      if (!value) return '-';
      return `<a href="mailto:${value}" class="text-info">${value}</a>`;
    },

    // Format pour les numéros de téléphone
    phone: (value: string) => {
      if (!value) return '-';
      // Format français : 01 23 45 67 89
      const formatted = value.replace(/(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})/, '$1 $2 $3 $4 $5');
      return `<a href="tel:${value}" class="text-info">${formatted}</a>`;
    },

    // Format pour les montants
    currency: (value: number, currency = '€') => {
      if (value == null) return '-';
      return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: currency === '€' ? 'EUR' : currency
      }).format(value);
    },

    // Format pour les pourcentages
    percentage: (value: number) => {
      if (value == null) return '-';
      return `${value.toFixed(2)}%`;
    },

    // Format pour les nombres avec séparateurs
    number: (value: number) => {
      if (value == null) return '-';
      return new Intl.NumberFormat('fr-FR').format(value);
    },

    // Format pour les textes longs (avec ellipsis)
    truncate: (value: string, maxLength = 50) => {
      if (!value) return '-';
      return value.length > maxLength 
        ? `<span title="${value}">${value.substring(0, maxLength)}...</span>`
        : value;
    },

    // Format pour les rôles/permissions
    roleList: (value: string[]) => {
      if (!value || value.length === 0) return '-';
      return value.map(role => `<span class="badge badge-primary">${role}</span>`).join(' ');
    },

    // Format pour les priorités
    priority: (value: 'low' | 'medium' | 'high') => {
      const badges = {
        low: '<span class="badge badge-success">Faible</span>',
        medium: '<span class="badge badge-warning">Moyenne</span>',
        high: '<span class="badge badge-danger">Élevée</span>'
      };
      return badges[value] || '-';
    },

    // Format pour les statuts génériques
    status: (value: string) => {
      const statusMap: Record<string, string> = {
        active: '<span class="badge badge-success">Actif</span>',
        inactive: '<span class="badge badge-secondary">Inactif</span>',
        pending: '<span class="badge badge-warning">En attente</span>',
        approved: '<span class="badge badge-success">Approuvé</span>',
        rejected: '<span class="badge badge-danger">Rejeté</span>',
        draft: '<span class="badge badge-info">Brouillon</span>',
        published: '<span class="badge badge-success">Publié</span>'
      };
      return statusMap[value?.toLowerCase()] || value || '-';
    }
  };

  /**
   * Actions prédéfinies courantes
   */
  readonly actionTemplates = {
    // Actions CRUD standard
    crud: `
      <button class="btn btn-sm btn-primary" (click)="edit($implicit)" title="Modifier">
        ✏️
      </button>
      <button class="btn btn-sm btn-danger" (click)="delete($implicit)" title="Supprimer">
        🗑️
      </button>
    `,

    // Actions avec visualisation
    viewEditDelete: `
      <button class="btn btn-sm btn-info" (click)="view($implicit)" title="Voir">
        👁️
      </button>
      <button class="btn btn-sm btn-primary" (click)="edit($implicit)" title="Modifier">
        ✏️
      </button>
      <button class="btn btn-sm btn-danger" (click)="delete($implicit)" title="Supprimer">
        🗑️
      </button>
    `,

    // Actions d'approbation
    approval: `
      <button class="btn btn-sm btn-success" (click)="approve($implicit)" title="Approuver">
        ✅
      </button>
      <button class="btn btn-sm btn-danger" (click)="reject($implicit)" title="Rejeter">
        ❌
      </button>
    `,

    // Toggle activation
    toggle: `
      <button 
        class="btn btn-sm"
        [class.btn-success]="!$implicit.isActive"
        [class.btn-warning]="$implicit.isActive"
        (click)="toggleActive($implicit)"
        [title]="$implicit.isActive ? 'Désactiver' : 'Activer'"
      >
        {{ $implicit.isActive ? '⏸️' : '▶️' }}
      </button>
    `
  };

  /**
   * Configurations prédéfinies pour différents types de listes
   */
  readonly presetConfigs = {
    // Configuration pour les listes d'utilisateurs
    users: {
      showSearch: true,
      showItemsPerPage: true,
      enablePagination: true,
      defaultItemsPerPage: 25,
      itemsPerPageOptions: [10, 25, 50, 100],
      striped: true,
      bordered: true,
      hover: true,
      searchPlaceholder: 'Rechercher par nom, prénom, email...'
    },

    // Configuration pour les listes de rôles/permissions
    roles: {
      showSearch: true,
      showItemsPerPage: true,
      enablePagination: true,
      defaultItemsPerPage: 15,
      itemsPerPageOptions: [15, 30, 50],
      striped: true,
      bordered: true,
      hover: true,
      searchPlaceholder: 'Rechercher par nom de rôle...'
    },

    // Configuration pour les listes de logs/historique
    logs: {
      showSearch: true,
      showItemsPerPage: true,
      enablePagination: true,
      defaultItemsPerPage: 50,
      itemsPerPageOptions: [25, 50, 100, 200],
      striped: true,
      bordered: false,
      hover: true,
      searchPlaceholder: 'Rechercher dans les logs...'
    },

    // Configuration pour les petites listes
    small: {
      showSearch: false,
      showItemsPerPage: false,
      enablePagination: false,
      striped: true,
      bordered: true,
      hover: true
    },

    // Configuration avec sélection multiple
    selectable: {
      showSearch: true,
      showItemsPerPage: true,
      enablePagination: true,
      enableSelection: true,
      selectionMode: 'multiple' as const,
      defaultItemsPerPage: 20,
      striped: true,
      bordered: true,
      hover: true
    }
  };

  /**
   * Génère des colonnes standard pour les entités courantes
   */
  generateColumns = {
    // Colonnes pour les utilisateurs
    user: () => [
      { key: 'id', label: 'ID', sortable: true, width: '60px', cssClass: 'text-center' },
      { key: 'name', label: 'Nom', sortable: true, filterable: true },
      { key: 'firstName', label: 'Prénom', sortable: true, filterable: true },
      { key: 'lastName', label: 'Nom de famille', sortable: true, filterable: true },
      { key: 'email', label: 'Email', sortable: true, filterable: true, formatter: this.formatters.email },
      { key: 'isActive', label: 'Statut', sortable: true, formatter: this.formatters.activeStatus, cssClass: 'text-center' },
      { key: 'lastEditAt', label: 'Modifié le', sortable: true, formatter: this.formatters.datetime },
      { key: 'lastEditBy', label: 'Modifié par', sortable: true }
    ],

    // Colonnes pour les rôles
    role: () => [
      { key: 'id', label: 'ID', sortable: true, width: '60px', cssClass: 'text-center' },
      { key: 'name', label: 'Nom', sortable: true, filterable: true },
      { key: 'description', label: 'Description', filterable: true, formatter: (val: string) => this.formatters.truncate(val, 100) },
      { key: 'isDeleted', label: 'Statut', sortable: true, formatter: (val: boolean) => val ? 'Supprimé' : 'Actif', cssClass: 'text-center' },
      { key: 'lastEditAt', label: 'Modifié le', sortable: true, formatter: this.formatters.datetime },
      { key: 'lastEditBy', label: 'Modifié par', sortable: true }
    ],

    // Colonnes pour les privilèges
    privilege: () => [
      { key: 'id', label: 'ID', sortable: true, width: '60px', cssClass: 'text-center' },
      { key: 'name', label: 'Nom', sortable: true, filterable: true },
      { key: 'description', label: 'Description', filterable: true, formatter: (val: string) => this.formatters.truncate(val, 80) },
      { key: 'isConfigurableRead', label: 'Lecture', sortable: true, formatter: this.formatters.yesNo, cssClass: 'text-center' },
      { key: 'isConfigurableAdd', label: 'Ajout', sortable: true, formatter: this.formatters.yesNo, cssClass: 'text-center' },
      { key: 'isConfigurableModify', label: 'Modification', sortable: true, formatter: this.formatters.yesNo, cssClass: 'text-center' },
      { key: 'isConfigurableDelete', label: 'Suppression', sortable: true, formatter: this.formatters.yesNo, cssClass: 'text-center' }
    ],

    // Colonnes pour les accès/badges
    access: () => [
      { key: 'id', label: 'ID', sortable: true, width: '60px', cssClass: 'text-center' },
      { key: 'badgeNumber', label: 'N° Badge', sortable: true, filterable: true },
      { key: 'name', label: 'Nom', sortable: true, filterable: true },
      { key: 'description', label: 'Description', filterable: true, formatter: (val: string) => this.formatters.truncate(val, 60) },
      { key: 'isDeleted', label: 'Statut', sortable: true, formatter: (val: boolean) => val ? 'Supprimé' : 'Actif', cssClass: 'text-center' },
      { key: 'lastEditAt', label: 'Modifié le', sortable: true, formatter: this.formatters.datetime },
      { key: 'lastEditBy', label: 'Modifié par', sortable: true }
    ],

    // Colonnes pour les associations (générique)
    association: (entity1Name: string, entity2Name: string) => [
      { key: 'id', label: 'ID', sortable: true, width: '60px', cssClass: 'text-center' },
      { key: 'entity1', label: entity1Name, sortable: true, filterable: true },
      { key: 'entity2', label: entity2Name, sortable: true, filterable: true },
      { key: 'lastEditAt', label: 'Modifié le', sortable: true, formatter: this.formatters.datetime },
      { key: 'lastEditBy', label: 'Modifié par', sortable: true }
    ]
  };
  /**
   * Utilitaire pour créer des filtres personnalisés
   */
  createCustomFilter<T>(filterFn: (item: T, searchTerm: string) => boolean) {
    return filterFn;
  }

  /**
   * Utilitaire pour créer des comparateurs de tri personnalisés
   */
  createCustomSort<T>(sortFn: (a: T, b: T) => number) {
    return sortFn;
  }

  /**
   * Validateurs pour les données de table
   */
  validators = {
    required: (value: unknown) => value != null && value !== '',
    email: (value: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value),
    phone: (value: string) => /^(\+33|0)[1-9](\d{8})$/.test(value.replace(/\s/g, '')),
    minLength: (value: string, min: number) => value && value.length >= min,
    maxLength: (value: string, max: number) => value && value.length <= max
  };

  /**
   * Exportation des données en CSV
   */
  exportToCSV<T>(data: T[], columns: {key: string; label: string}[], filename = 'export.csv'): void {
    const headers = columns.map(col => col.label).join(',');
    const rows = data.map(item => 
      columns.map(col => {
        const value = this.getNestedValue(item, col.key);
        // Nettoyer la valeur pour CSV (enlever HTML, guillemets, etc.)
        const cleanValue = String(value || '').replace(/"/g, '""').replace(/<[^>]*>/g, '');
        return `"${cleanValue}"`;
      }).join(',')
    );

    const csv = [headers, ...rows].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    
    if (link.download !== undefined) {
      const url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', filename);
      link.style.visibility = 'hidden';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

  /**
   * Exportation des données en Excel
   */
  exportToExcel<T>(data: T[], columns: {key: string; label: string}[], filename = 'export.xlsx', sheetName = 'Data'): void {
    const workbook = XLSX.utils.book_new();
    
    // Préparer les données avec les en-têtes
    const headers = columns.map(col => col.label);
    const rows = data.map(item => 
      columns.map(col => {
        const value = this.getNestedValue(item, col.key);
        // Nettoyer la valeur (enlever HTML, etc.)
        return String(value || '').replace(/<[^>]*>/g, '');
      })
    );
    
    const worksheetData = [headers, ...rows];
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    
    // Ajuster la largeur des colonnes
    const colWidths = columns.map((col, index) => {
      const columnValues = worksheetData.map(row => String(row[index] || ''));
      const maxLength = Math.max(...columnValues.map(val => val.length));
      return { wch: Math.min(Math.max(maxLength + 2, col.label.length + 2), 50) };
    });
    worksheet['!cols'] = colWidths;
    
    XLSX.utils.book_append_sheet(workbook, worksheet, sheetName);
    XLSX.writeFile(workbook, filename);
  }

  /**
   * Exportation des données en PDF
   */
  exportToPDF<T>(data: T[], columns: {key: string; label: string}[], filename = 'export.pdf', title = 'Rapport'): void {
    const doc = new jsPDF();
    
    // Titre du document
    doc.setFontSize(16);
    doc.text(title, 14, 22);
    
    // Date de génération
    doc.setFontSize(10);
    doc.text(`Généré le: ${new Date().toLocaleDateString('fr-FR')}`, 14, 30);
    
    // Préparer les données pour le tableau
    const headers = columns.map(col => col.label);
    const rows = data.map(item => 
      columns.map(col => {
        const value = this.getNestedValue(item, col.key);
        // Nettoyer et formater la valeur
        return String(value || '').replace(/<[^>]*>/g, '').substring(0, 50);
      })
    );
    
    // Générer le tableau
    (doc as any).autoTable({
      head: [headers],
      body: rows,
      startY: 40,
      styles: {
        fontSize: 8,
        cellPadding: 3,
      },
      headStyles: {
        fillColor: [41, 128, 185],
        textColor: 255,
      },
      columnStyles: {
        // Ajuster les largeurs de colonnes si nécessaire
      },
      margin: { top: 40 },
    });
    
    doc.save(filename);
  }

  /**
   * Génération d'un rapport Excel avec plusieurs feuilles
   */
  exportMultiSheetExcel(sheets: {name: string; data: any[]; columns: {key: string; label: string}[]}[], filename = 'rapport.xlsx'): void {
    const workbook = XLSX.utils.book_new();
    
    sheets.forEach(sheet => {
      const headers = sheet.columns.map(col => col.label);
      const rows = sheet.data.map(item => 
        sheet.columns.map(col => {
          const value = this.getNestedValue(item, col.key);
          return String(value || '').replace(/<[^>]*>/g, '');
        })
      );
      
      const worksheetData = [headers, ...rows];
      const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
      
      // Ajuster la largeur des colonnes
      const colWidths = sheet.columns.map((col, index) => {
        const columnValues = worksheetData.map(row => String(row[index] || ''));
        const maxLength = Math.max(...columnValues.map(val => val.length));
        return { wch: Math.min(Math.max(maxLength + 2, col.label.length + 2), 50) };
      });
      worksheet['!cols'] = colWidths;
      
      XLSX.utils.book_append_sheet(workbook, worksheet, sheet.name);
    });
    
    XLSX.writeFile(workbook, filename);
  }

  /**
   * Génération d'un rapport PDF avec statistiques
   */
  exportReportToPDF(reportData: {
    title: string;
    summary: {[key: string]: any};
    tables: {title: string; data: any[]; columns: {key: string; label: string}[]}[];
  }, filename = 'rapport.pdf'): void {
    const doc = new jsPDF();
    let currentY = 20;
    
    // Titre principal
    doc.setFontSize(18);
    doc.text(reportData.title, 14, currentY);
    currentY += 15;
    
    // Date de génération
    doc.setFontSize(10);
    doc.text(`Généré le: ${new Date().toLocaleDateString('fr-FR')} à ${new Date().toLocaleTimeString('fr-FR')}`, 14, currentY);
    currentY += 20;
    
    // Résumé/statistiques
    if (reportData.summary && Object.keys(reportData.summary).length > 0) {
      doc.setFontSize(14);
      doc.text('Résumé:', 14, currentY);
      currentY += 10;
      
      doc.setFontSize(10);
      Object.entries(reportData.summary).forEach(([key, value]) => {
        doc.text(`${key}: ${value}`, 20, currentY);
        currentY += 7;
      });
      currentY += 10;
    }
    
    // Tableaux
    reportData.tables.forEach(table => {
      // Vérifier si on a assez d'espace, sinon nouvelle page
      if (currentY > 250) {
        doc.addPage();
        currentY = 20;
      }
      
      doc.setFontSize(12);
      doc.text(table.title, 14, currentY);
      currentY += 10;
      
      const headers = table.columns.map(col => col.label);
      const rows = table.data.map(item => 
        table.columns.map(col => {
          const value = this.getNestedValue(item, col.key);
          return String(value || '').replace(/<[^>]*>/g, '').substring(0, 40);
        })
      );
      
      (doc as any).autoTable({
        head: [headers],
        body: rows,
        startY: currentY,
        styles: {
          fontSize: 8,
          cellPadding: 2,
        },
        headStyles: {
          fillColor: [52, 73, 94],
          textColor: 255,
        },
        margin: { top: 10, bottom: 10 },
      });
      
      currentY = (doc as any).lastAutoTable.finalY + 15;
    });
    
    doc.save(filename);
  }

  /**
   * Récupère une valeur nested dans un objet
   */  private getNestedValue(obj: unknown, path: string): unknown {
    return path.split('.').reduce((current: unknown, key: string) => 
      (current && typeof current === 'object' && current !== null) 
        ? (current as Record<string, unknown>)[key] 
        : undefined, 
      obj
    );
  }
}
