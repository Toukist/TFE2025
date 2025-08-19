import { Component, Input, Output, EventEmitter, TemplateRef, OnDestroy, computed, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

export interface DataTableColumn<T = unknown> {
  key: string;
  label: string;
  sortable?: boolean;
  filterable?: boolean;
  width?: string;
  template?: TemplateRef<{ $implicit: T; column: DataTableColumn<T>; value: unknown }>;
  formatter?: (value: unknown, item: T) => string;
  cssClass?: string;
}

export interface DataTableConfig {
  searchPlaceholder?: string;
  noDataMessage?: string;
  showSearch?: boolean;
  showItemsPerPage?: boolean;
  itemsPerPageOptions?: number[];
  defaultItemsPerPage?: number;
  enablePagination?: boolean;
  enableSelection?: boolean;
  selectionMode?: 'single' | 'multiple';
  striped?: boolean;
  bordered?: boolean;
  hover?: boolean;
}

export interface PaginationInfo {
  page: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="data-table-wrapper">
      <!-- En-tête avec recherche et contrôles -->
      <div class="table-header" *ngIf="config.showSearch || config.showItemsPerPage">
        <div class="search-container" *ngIf="config.showSearch">
          <input
            type="text"
            class="search-input"
            [placeholder]="config.searchPlaceholder || 'Rechercher...'"
            [(ngModel)]="searchTerm"
            (input)="onSearchChange($event)"
          />
          <span class="search-icon">🔍</span>
        </div>
        
        <div class="items-per-page" *ngIf="config.showItemsPerPage && config.enablePagination">
          <label>
            Afficher
            <select [(ngModel)]="itemsPerPage" (change)="onItemsPerPageChange()">
              <option *ngFor="let option of config.itemsPerPageOptions" [value]="option">
                {{ option }}
              </option>
            </select>
            éléments
          </label>
        </div>
      </div>

      <!-- Indicateur de chargement -->
      <div *ngIf="loading" class="loading-indicator">
        <div class="spinner"></div>
        <span>Chargement...</span>
      </div>

      <!-- Message d'erreur -->
      <div *ngIf="error" class="error-message">
        <span class="error-icon">⚠️</span>
        <span>{{ error }}</span>
      </div>

      <!-- Tableau -->
      <div class="table-container" *ngIf="!loading">
        <table 
          class="data-table"
          [class.striped]="config.striped"
          [class.bordered]="config.bordered"
          [class.hover]="config.hover"
        >
          <thead>
            <tr>
              <!-- Colonne de sélection -->
              <th *ngIf="config.enableSelection" class="selection-column">
                <input
                  *ngIf="config.selectionMode === 'multiple'"
                  type="checkbox"
                  [checked]="allSelected()"
                  [indeterminate]="someSelected()"
                  (change)="toggleAllSelection()"
                />
              </th>
              
              <!-- Colonnes de données -->
              <th 
                *ngFor="let column of columns"
                [class]="column.cssClass"
                [style.width]="column.width"
                [class.sortable]="column.sortable"
                (click)="column.sortable && sort(column.key)"
              >
                <span>{{ column.label }}</span>
                <span 
                  *ngIf="column.sortable && sortColumn === column.key"
                  class="sort-indicator"
                  [class.asc]="sortDirection === 'asc'"
                  [class.desc]="sortDirection === 'desc'"
                >
                  {{ sortDirection === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              
              <!-- Colonne actions -->
              <th *ngIf="actionsTemplate" class="actions-column">Actions</th>
            </tr>
          </thead>
          
          <tbody>
            <tr *ngFor="let item of displayedData(); let i = index">
              <!-- Sélection -->
              <td *ngIf="config.enableSelection" class="selection-column">
                <input
                  type="checkbox"
                  [checked]="isSelected(item)"
                  (change)="toggleSelection(item)"
                />
              </td>
              
              <!-- Données -->
              <td 
                *ngFor="let column of columns"
                [class]="column.cssClass"
              >
                <!-- Template personnalisé -->
                <ng-container *ngIf="column.template; else defaultCell">
                  <ng-container 
                    *ngTemplateOutlet="column.template; context: { $implicit: item, column: column, value: getColumnValue(item, column.key) }"
                  ></ng-container>
                </ng-container>
                
                <!-- Cellule par défaut -->
                <ng-template #defaultCell>
                  {{ getFormattedValue(item, column) }}
                </ng-template>
              </td>
              
              <!-- Actions -->
              <td *ngIf="actionsTemplate" class="actions-column">
                <ng-container 
                  *ngTemplateOutlet="actionsTemplate; context: { $implicit: item, index: i }"
                ></ng-container>
              </td>
            </tr>
            
            <!-- Message si aucune donnée -->
            <tr *ngIf="displayedData().length === 0">
              <td [attr.colspan]="totalColumns()" class="no-data">
                <span class="no-data-icon">📋</span>
                <span>{{ config.noDataMessage || 'Aucune donnée disponible' }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="pagination-container" *ngIf="config.enablePagination && !loading && displayedData().length > 0">
        <div class="pagination-info">
          Affichage de {{ startItem() }} à {{ endItem() }} sur {{ filteredData().length }} éléments
        </div>
        
        <div class="pagination-controls">          <button 
            class="pagination-btn"
            [disabled]="currentPage() === 1"
            (click)="goToPage(1)"
            title="Première page"
          >
            ⏮
          </button>
          
          <button 
            class="pagination-btn"
            [disabled]="currentPage() === 1"
            (click)="goToPage(currentPage() - 1)"
            title="Page précédente"
          >
            ◀
          </button>
          
          <div class="page-numbers">
            <button
              *ngFor="let page of getVisiblePages()"
              class="pagination-btn page-number"
              [class.active]="page === currentPage()"
              (click)="goToPage(page)"
            >
              {{ page }}
            </button>
          </div>
          
          <button 
            class="pagination-btn"
            [disabled]="currentPage() === totalPages()"
            (click)="goToPage(currentPage() + 1)"
            title="Page suivante"
          >
            ▶
          </button>
          
          <button 
            class="pagination-btn"
            [disabled]="currentPage() === totalPages()"
            (click)="goToPage(totalPages())"
            title="Dernière page"
          >
            ⏭
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent<T = Record<string, unknown>> implements OnDestroy {
  @Input({ required: true }) data = signal<T[]>([]);
  @Input({ required: true }) columns: DataTableColumn<T>[] = [];
  @Input() config: DataTableConfig = {};
  @Input() loading = false;
  @Input() error: string | null = null;
  @Input() actionsTemplate?: TemplateRef<{ $implicit: T; index: number }>;
  @Input() selectedItems = signal<T[]>([]);

  @Output() selectionChange = new EventEmitter<T[]>();
  @Output() sortChange = new EventEmitter<{ column: string; direction: 'asc' | 'desc' }>();
  @Output() pageChange = new EventEmitter<PaginationInfo>();
  @Output() searchChange = new EventEmitter<string>();

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // État de recherche et tri
  searchTerm = '';
  sortColumn = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // État de pagination
  currentPage = signal(1);
  itemsPerPage = signal(10);

  constructor() {
    // Configuration par défaut
    this.config = {
      searchPlaceholder: 'Rechercher...',
      noDataMessage: 'Aucune donnée disponible',
      showSearch: true,
      showItemsPerPage: true,
      itemsPerPageOptions: [5, 10, 25, 50, 100],
      defaultItemsPerPage: 10,
      enablePagination: true,
      enableSelection: false,
      selectionMode: 'single',
      striped: true,
      bordered: true,
      hover: true,
      ...this.config
    };

    this.itemsPerPage.set(this.config.defaultItemsPerPage || 10);

    // Gestion de la recherche avec debounce
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(term => {
        this.searchTerm = term;
        this.currentPage.set(1);
        this.searchChange.emit(term);
      });

    // Réinitialiser la page lors du changement de données
    effect(() => {
      if (this.data().length > 0) {
        this.currentPage.set(1);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Données filtrées par la recherche
  filteredData = computed(() => {
    const data = this.data();
    if (!this.searchTerm.trim()) {
      return data;
    }

    const searchLower = this.searchTerm.toLowerCase().trim();
    return data.filter(item => {
      return this.columns.some(column => {
        if (!column.filterable && column.filterable !== undefined) {
          return false;
        }
        
        const value = this.getColumnValue(item, column.key);
        return String(value).toLowerCase().includes(searchLower);
      });
    });
  });

  // Données triées
  sortedData = computed(() => {
    const data = [...this.filteredData()];
    if (!this.sortColumn) {
      return data;
    }    return data.sort((a, b) => {
      const aValue = this.getColumnValue(a, this.sortColumn);
      const bValue = this.getColumnValue(b, this.sortColumn);
      
      let comparison = 0;
      if (aValue != null && bValue != null) {
        if (aValue < bValue) comparison = -1;
        if (aValue > bValue) comparison = 1;
      } else if (aValue == null && bValue != null) {
        comparison = -1;
      } else if (aValue != null && bValue == null) {
        comparison = 1;
      }
      
      return this.sortDirection === 'asc' ? comparison : -comparison;
    });
  });

  // Données affichées (avec pagination)
  displayedData = computed(() => {
    const data = this.sortedData();
    if (!this.config.enablePagination) {
      return data;
    }

    const start = (this.currentPage() - 1) * this.itemsPerPage();
    const end = start + this.itemsPerPage();
    return data.slice(start, end);
  });

  // Calculs de pagination
  totalPages = computed(() => 
    Math.ceil(this.filteredData().length / this.itemsPerPage())
  );

  startItem = computed(() => 
    this.filteredData().length === 0 ? 0 : (this.currentPage() - 1) * this.itemsPerPage() + 1
  );

  endItem = computed(() => 
    Math.min(this.currentPage() * this.itemsPerPage(), this.filteredData().length)
  );

  totalColumns = computed(() => {
    let count = this.columns.length;
    if (this.config.enableSelection) count++;
    if (this.actionsTemplate) count++;
    return count;
  });  // Méthodes utilitaires
  getColumnValue(item: T, key: string): unknown {
    return key.split('.').reduce((obj: unknown, k: string) => (obj as Record<string, unknown>)?.[k], item);
  }

  getFormattedValue(item: T, column: DataTableColumn<T>): string {
    const value = this.getColumnValue(item, column.key);
    if (column.formatter) {
      return column.formatter(value, item);
    }
    return value != null ? String(value) : '';
  }

  // Gestion de la recherche
  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }

  // Gestion du tri
  sort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    
    this.sortChange.emit({ column, direction: this.sortDirection });
  }

  // Gestion de la pagination
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.pageChange.emit({
        page,
        itemsPerPage: this.itemsPerPage(),
        totalItems: this.filteredData().length,
        totalPages: this.totalPages()
      });
    }
  }

  onItemsPerPageChange(): void {
    this.currentPage.set(1);
    this.pageChange.emit({
      page: 1,
      itemsPerPage: this.itemsPerPage(),
      totalItems: this.filteredData().length,
      totalPages: this.totalPages()
    });
  }

  getVisiblePages(): number[] {
    const current = this.currentPage();
    const total = this.totalPages();
    const delta = 2;
    
    const range: number[] = [];
    const rangeWithDots: (number | string)[] = [];
    
    for (let i = Math.max(2, current - delta); 
         i <= Math.min(total - 1, current + delta); 
         i++) {
      range.push(i);
    }
    
    if (current - delta > 2) {
      rangeWithDots.push(1, '...');
    } else {
      rangeWithDots.push(1);
    }
    
    rangeWithDots.push(...range);
    
    if (current + delta < total - 1) {
      rangeWithDots.push('...', total);
    } else {
      rangeWithDots.push(total);
    }
    
    return rangeWithDots.filter((page): page is number => typeof page === 'number');
  }

  // Gestion de la sélection
  isSelected(item: T): boolean {
    return this.selectedItems().includes(item);
  }

  toggleSelection(item: T): void {
    const currentSelection = [...this.selectedItems()];
    const index = currentSelection.indexOf(item);
    
    if (this.config.selectionMode === 'single') {
      this.selectedItems.set(index === -1 ? [item] : []);
    } else {
      if (index === -1) {
        currentSelection.push(item);
      } else {
        currentSelection.splice(index, 1);
      }
      this.selectedItems.set(currentSelection);
    }
    
    this.selectionChange.emit(this.selectedItems());
  }

  toggleAllSelection(): void {
    if (this.allSelected()) {
      this.selectedItems.set([]);
    } else {
      this.selectedItems.set([...this.displayedData()]);
    }
    this.selectionChange.emit(this.selectedItems());
  }

  allSelected(): boolean {
    const displayed = this.displayedData();
    return displayed.length > 0 && displayed.every(item => this.isSelected(item));
  }

  someSelected(): boolean {
    const displayed = this.displayedData();
    return displayed.some(item => this.isSelected(item)) && !this.allSelected();
  }
}
