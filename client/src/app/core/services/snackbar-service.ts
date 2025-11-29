import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  private snackbar = inject(MatSnackBar);

  error(message: string) {
    // When openning a snackbar we need to pass it the text we want to display, another string for specifying the action (in this case we want a close button), and a configuration object. In the object we have a panelClass key that allows us to specify a class name that will be applied to the snackbar. The classes will be defined in the style.scss file
    this.snackbar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snack-error'],
    });
  }
  success(message: string) {
    this.snackbar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snack-success'],
    });
  }
}
