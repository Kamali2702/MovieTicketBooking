
import { Routes } from '@angular/router';
import { Home } from './pages/home/home';

export const routes: Routes = [
  {
    path: '',
    component: Home,
    title: 'Home | Movie Ticket Booking'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
