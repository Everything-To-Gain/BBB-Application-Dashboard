import { Component, input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { HlmError, HlmFormField } from '@spartan-ng/helm/form-field';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmLabel } from '@spartan-ng/helm/label';

@Component({
  selector: 'app-business-info-section',
  imports: [HlmFormField, HlmInput, HlmError, HlmLabel, ...HlmCardImports],
  templateUrl: './business-info-section.component.html',
})
export class BusinessInfoSectionComponent {
  form = input.required<FormGroup>();
}
