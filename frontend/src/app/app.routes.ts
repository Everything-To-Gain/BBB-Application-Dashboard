import { Routes } from '@angular/router';
import { AccreditationFormComponent } from './features/accreditation-form/pages/accreditation-form.component';

export const routes: Routes = [
  {
    path: '',
    title: 'Home',
    redirectTo: 'accreditation-form',
    pathMatch: 'full',
  },
  {
    title: 'Accreditation Form',
    path: 'accreditation-form',
    component: AccreditationFormComponent,
  },
];
