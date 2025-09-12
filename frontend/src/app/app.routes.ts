import { Routes } from '@angular/router';
import { AccreditationFormComponent } from './features/accreditation-form/pages/accreditation-form.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'accreditation-form',
    pathMatch: 'full',
  },
  {
    path: 'accreditation-form',
    component: AccreditationFormComponent,
  },
];
