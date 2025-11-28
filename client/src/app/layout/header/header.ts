import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';

@Component({
  selector: 'app-header',
  imports: [MatIcon, MatButtonModule, MatBadge],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {}
