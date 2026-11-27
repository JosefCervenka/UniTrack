import {Injectable} from '@angular/core';
import {ApiValidationError} from '../Models/error-model';
import {CreateLostItem} from '../Models/create-lost-item';
import {ReadLostItem} from '../Models/read-lost-item';
import {UpdateLostItem} from '../Models/update-lost-item';

@Injectable({
  providedIn: 'root'
})
export class LostItemServiceService {
  async getAll(skip: number, take: number, search: string): Promise<ReadLostItem[] | ApiValidationError[]> {
    const query = new URLSearchParams({
      skip: skip.toString(),
      take: take.toString(),
      search
    });
    const response = await fetch(`/api/lost-items?${query}`, {
      credentials: 'include'
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }

    return await response.json() as ReadLostItem[];
  }

  async delete(id: string): Promise<ApiValidationError[] | void> {
    const response = await fetch(`/api/lost-items/${encodeURIComponent(id)}`, {
      method: 'DELETE',
      credentials: 'include'
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }
  }

  async update(item: UpdateLostItem): Promise<ApiValidationError[] | void> {
    const response = await fetch('/api/lost-items', {
      method: 'PUT',
      headers: {'Content-Type': 'application/json'},
      credentials: 'include',
      body: JSON.stringify(item)
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }
  }

  async create(item: CreateLostItem): Promise<ApiValidationError[] | void> {
    const formData = new FormData();
    formData.append('Name', item.name);
    formData.append('Description', item.description);
    formData.append('LostLocation', item.lostLocation);
    if (item.image) {
      formData.append('Image', item.image);
    }

    const response = await fetch('/api/lost-items', {
      method: 'POST',
      credentials: 'include',
      body: formData
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }
  }

  private async extractErrors(response: Response): Promise<ApiValidationError[]> {
    try {
      const data: unknown = await response.json();
      if (Array.isArray(data)) {
        return data as ApiValidationError[];
      }
    } catch {
      console.error('Response was not valid JSON');
    }

    return [
      {
        field: '',
        message: response.statusText || `Request failed (${response.status})`,
        errorCode: response.status,
        state: null
      }
    ];
  }
}