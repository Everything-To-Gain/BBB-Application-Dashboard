import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccreditationFormComponent } from './features/accreditation-form/accreditation-form.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AccreditationFormComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'bbb-application-angular';
}
