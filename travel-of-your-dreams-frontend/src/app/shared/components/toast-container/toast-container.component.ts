import { Component } from '@angular/core';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="toast-stack">
      @for (item of notifications.items(); track item.id) {
        <button class="toast" [class.error]="item.type === 'error'" type="button" (click)="notifications.remove(item.id)">
          {{ item.message }}
        </button>
      }
    </div>
  `
})
export class ToastContainerComponent {
  constructor(readonly notifications: NotificationService) {}
}
