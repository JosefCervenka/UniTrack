import {Component, computed, inject, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Router} from '@angular/router';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {StatusDropdownComponent} from '../../Components/status-dropdown/status-dropdown.component';
import {ApiValidationError} from '../../Models/error-model';
import {CreateLostItem} from '../../Models/create-lost-item';
import {LostItemServiceService} from '../../Services/lost-item-service.service';

const ITEM_FIELDS: (keyof CreateLostItem)[] = ['name', 'description', 'lostLocation', 'statusId'];
const MAX_IMAGE_SIZE = 5 * 1024 * 1024;

@Component({
  selector: 'app-create-lost-item',
  standalone: true,
  imports: [FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, StatusDropdownComponent],
  templateUrl: './create-lost-item.component.html',
  styleUrl: './create-lost-item.component.scss'
})
export class CreateLostItemComponent {
  private readonly lostItemService = inject(LostItemServiceService);
  private readonly router = inject(Router);

  readonly model: CreateLostItem = {
    name: '',
    description: '',
    lostLocation: '',
    statusId: 1,
    image: null
  };
  readonly errors = signal<ApiValidationError[]>([]);
  readonly success = signal(false);
  readonly submitting = signal(false);
  readonly generalError = computed(() =>
    this.errors().find(error => !ITEM_FIELDS.some(field => field.toLowerCase() === error.field.toLowerCase()))?.message
  );
  imageName = '';

  fieldError(fieldName: string): string | undefined {
    return this.errors().find(error => error.field.toLowerCase() === fieldName.toLowerCase())?.message;
  }

  async onImageSelected(event: Event): Promise<void> {
    this.errors.update(errors => errors.filter(error => error.field.toLowerCase() !== 'image'));
    this.model.image = null;
    this.imageName = '';

    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.setImageError('Please select an image file.');
      input.value = '';
      return;
    }

    if (file.size > MAX_IMAGE_SIZE) {
      this.setImageError('Image size must not exceed 5 MB.');
      input.value = '';
      return;
    }

    try {
      this.model.image = await this.readImageAsBase64(file);
      this.imageName = file.name;
    } catch (error) {
      console.error('Could not read selected image', error);
      this.setImageError('Could not read the selected image.');
      input.value = '';
    }
  }

  async onSubmit(): Promise<void> {
    if (this.submitting()) {
      return;
    }

    this.errors.set([]);
    this.success.set(false);
    this.submitting.set(true);

    try {
      const result = await this.lostItemService.create(this.model);
      if (result) {
        this.errors.set(result);
        return;
      }

      this.success.set(true);
      await this.router.navigate(['/search-lost-items']);
    } catch (error) {
      console.error('Could not create lost item', error);
      this.errors.set([{
        field: '',
        message: 'Could not create the lost item. Please try again.',
        errorCode: 0,
        state: null
      }]);
    } finally {
      this.submitting.set(false);
    }
  }

  private readImageAsBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        if (typeof reader.result !== 'string') {
          reject(new Error('Unexpected image data.'));
          return;
        }

        const base64 = reader.result.split(',')[1];
        if (!base64) {
          reject(new Error('Image data is empty.'));
          return;
        }
        resolve(base64);
      };
      reader.onerror = () => reject(reader.error ?? new Error('Image reading failed.'));
      reader.readAsDataURL(file);
    });
  }

  private setImageError(message: string): void {
    this.errors.update(errors => [
      ...errors,
      {field: 'image', message, errorCode: 400, state: null}
    ]);
  }
}
