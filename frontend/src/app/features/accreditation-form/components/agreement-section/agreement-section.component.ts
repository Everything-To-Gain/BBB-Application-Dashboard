import { Component, input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideShield, lucideCircleCheck } from '@ng-icons/lucide';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { HlmFormField, HlmError } from '@spartan-ng/helm/form-field';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmCheckbox } from '@spartan-ng/helm/checkbox';
import { HlmLabel } from '@spartan-ng/helm/label';

@Component({
  selector: 'app-agreement-section',
  imports: [NgIcon, HlmCardImports, HlmFormField, HlmInput, HlmError, HlmCheckbox, HlmLabel],
  providers: [provideIcons({ lucideShield, lucideCircleCheck })],
  templateUrl: './agreement-section.component.html',
})
export class AgreementSectionComponent {
  form = input.required<FormGroup>();
}
