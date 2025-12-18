import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialog } from '../../shared/components/confirmation-dialog/confirmation-dialog';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  private dialog = inject(MatDialog);

  confirm(title: string, message: string) {
    const dialogRef = this.dialog.open(ConfirmationDialog, {
      width: '400px',
      data: { title, message },
    });

    // Returning a promise
    return firstValueFrom(dialogRef.afterClosed()); // Returning true if confirmed or nothing if not confirmed
  }
}
