import {Component, inject, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {DatePipe} from '@angular/common';
import {ApiValidationError} from '../../Models/error-model';
import {ReadLostItem} from '../../Models/read-lost-item';
import {LostItemServiceService} from '../../Services/lost-item-service.service';

const PAGE_SIZE = 12;

@Component({
  selector: 'app-search-lost-items',
  standalone: true,
  imports: [DatePipe, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './search-lost-items.component.html',
  styleUrl: './search-lost-items.component.scss'
})
export class SearchLostItemsComponent {
  private readonly lostItemService = inject(LostItemServiceService);

  search = '';
  readonly items = signal<ReadLostItem[]>([]);
  readonly errors = signal<ApiValidationError[]>([]);
  readonly loading = signal(false);
  readonly hasSearched = signal(false);
  readonly hasMore = signal(false);
  readonly unavailablePictures = signal<ReadonlySet<string>>(new Set());
  private offset = 0;
  private activeSearch = '';

  async onSearch(): Promise<void> {
    const query = this.search.trim();
    if (!query || this.loading()) {
      return;
    }

    this.activeSearch = query;
    this.offset = 0;
    this.items.set([]);
    this.errors.set([]);
    this.hasSearched.set(true);
    this.hasMore.set(false);
    await this.loadItems(false);
  }

  async loadMore(): Promise<void> {
    if (this.loading() || !this.hasMore()) {
      return;
    }

    await this.loadItems(true);
  }

  hidePicture(id: string): void {
    this.unavailablePictures.update(current => new Set(current).add(id));
  }

  private async loadItems(append: boolean): Promise<void> {
    this.loading.set(true);
    this.errors.set([]);

    try {
      const result = await this.lostItemService.getAll(this.offset, PAGE_SIZE, this.activeSearch);
      if (this.isValidationErrors(result)) {
        this.errors.set(result);
        this.hasMore.set(false);
        return;
      }

      this.items.update(current => append ? [...current, ...result] : result);
      this.offset += result.length;
      this.hasMore.set(result.length === PAGE_SIZE);
    } catch (error) {
      console.error('Could not load lost items', error);
      this.errors.set([{
        field: '',
        message: 'Could not load lost items. Please try again.',
        errorCode: 0,
        state: null
      }]);
      this.hasMore.set(false);
    } finally {
      this.loading.set(false);
    }
  }

  private isValidationErrors(result: ReadLostItem[] | ApiValidationError[]): result is ApiValidationError[] {
    return result.length > 0 && 'message' in result[0];
  }
}
