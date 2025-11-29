import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatCard } from '@angular/material/card';

@Component({
  selector: 'app-server-error',
  imports: [MatCard],
  templateUrl: './server-error.html',
  styleUrl: './server-error.scss',
})
export class ServerError {
  error?: any; // This canonly be initialized in the constructor so we have to add the ? sign
  // NOTE that the component will be able to access extra data passed to it only from the constructor!!!
  constructor(private router: Router) {
    const navigation = this.router.currentNavigation(); // Signal of the current Navigation object when the router is navigating, and null when idle. Note: The current navigation becomes to null after the NavigationEnd event is emitted.
    this.error = navigation?.extras.state?.['error']; // Setting the error to what we are passing through the errors interceptor
  }
}
