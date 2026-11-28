import {Injectable} from '@angular/core';
import {ApiValidationError} from '../Models/error-model';
import {ReadStatus} from '../Models/read-status';

@Injectable({
  providedIn: 'root'
})
export class StatusServiceService {
  async getAll(): Promise<ReadStatus[] | ApiValidationError[]> {
    const response = await fetch('/api/status', {
      credentials: 'include'
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }

    return await response.json() as ReadStatus[];
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