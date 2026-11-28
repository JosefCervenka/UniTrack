import {Component, EventEmitter, Input, OnInit, Output, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import {ApiValidationError} from '../../Models/error-model';
import {ReadStatus} from '../../Models/read-status';
import {StatusServiceService} from '../../Services/status-service.service';

@Component({
  selector: 'app-status-dropdown',
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './status-dropdown.component.html',
  styleUrl: './status-dropdown.component.scss'
})
export class StatusDropdownComponent implements OnInit {
  @Input() selectedStatusId: number | null = null;
  @Input() disabled = false;
  @Input() allowedStatusIds: number[] | null = null;
  @Output() readonly selectedStatusIdChange = new EventEmitter<number | null>();

  readonly statuses = signal<ReadStatus[]>([]);
  readonly error = signal<string | null>(null);
  readonly loading = signal(false);

  constructor(private readonly statusService: StatusServiceService) {}

  async ngOnInit(): Promise<void> {
    this.loading.set(true);

    try {
      const result = await this.statusService.getAll();
      if (this.isErrorResponse(result)) {
        this.error.set(result.map(item => item.message).join(' '));
      } else {
        const allowedStatusIds = this.allowedStatusIds;
        const statuses = allowedStatusIds
          ? result.filter(status => allowedStatusIds.includes(status.id))
          : result;
        this.statuses.set(statuses);
      }
    } catch (error) {
      console.error('Could not load statuses', error);
      this.error.set('Could not load statuses. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  onSelectionChange(statusId: number | null): void {
    this.selectedStatusIdChange.emit(statusId);
  }

  private isErrorResponse(result: ReadStatus[] | ApiValidationError[]): result is ApiValidationError[] {
    return result.length > 0 && 'message' in result[0];
  }
}
