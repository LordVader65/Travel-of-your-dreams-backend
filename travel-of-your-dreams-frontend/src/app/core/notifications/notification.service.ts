import { Injectable, signal } from '@angular/core';

export interface AppNotification {
  id: number;
  type: 'success' | 'error';
  message: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly items = signal<AppNotification[]>([]);
  private nextId = 1;

  success(message: string) {
    this.push('success', message);
  }

  error(message: string) {
    this.push('error', message);
  }

  remove(id: number) {
    this.items.update((items) => items.filter((item) => item.id !== id));
  }

  private push(type: AppNotification['type'], message: string) {
    const id = this.nextId++;
    this.items.update((items) => [...items, { id, type, message }]);
    window.setTimeout(() => this.remove(id), 4500);
  }
}
