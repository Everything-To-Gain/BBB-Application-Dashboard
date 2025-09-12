import { Component, input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmHint, HlmError } from '@spartan-ng/helm/form-field';
import { HlmFormField } from '@spartan-ng/helm/form-field';
import { BrnSelectImports } from '@spartan-ng/brain/select';
import { HlmSelectImports } from '@spartan-ng/helm/select';
import { HlmLabel } from '@spartan-ng/helm/label';
import { HlmCardImports } from '@spartan-ng/helm/card';
@Component({
  selector: 'app-business-details-section',
  imports: [
    HlmInput,
    HlmLabel,
    HlmCardImports,
    HlmError,
    HlmFormField,
    BrnSelectImports,
    HlmSelectImports,
  ],
  templateUrl: './business-details-section.component.html',
})
export class BusinessDetailsSectionComponent {
  form = input.required<FormGroup>();
}
