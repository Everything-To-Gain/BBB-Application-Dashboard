import { Injectable } from '@angular/core';

export interface Toast {
  title: string;
  description?: string;
  type: 'success' | 'error' | 'warning' | 'info';
  duration?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private toasts: Toast[] = [];

  show(toast: Toast): void {
    this.toasts.push(toast);

    // Auto remove after duration (default 5 seconds)
    const duration = toast.duration || 5000;
    setTimeout(() => {
      this.remove(toast);
    }, duration);

    // Log to console for now (in a real app, you'd show a toast UI)
    console.log(`Toast: ${toast.title}`, toast.description);
  }

  remove(toast: Toast): void {
    const index = this.toasts.indexOf(toast);
    if (index > -1) {
      this.toasts.splice(index, 1);
    }
  }

  getToasts(): Toast[] {
    return this.toasts;
  }

  clear(): void {
    this.toasts = [];
  }
}
