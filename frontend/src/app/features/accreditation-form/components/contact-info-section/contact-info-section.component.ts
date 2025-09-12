import { Component, input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BrnSelectImports } from '@spartan-ng/brain/select';
import { HlmSelectImports } from '@spartan-ng/helm/select';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { HlmError, HlmFormField } from '@spartan-ng/helm/form-field';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmLabel } from '@spartan-ng/helm/label';
import { HlmDatePicker } from '@spartan-ng/helm/date-picker';

@Component({
  selector: 'app-contact-info-section',
  imports: [
    HlmFormField,
    HlmInput,
    HlmError,
    HlmLabel,
    HlmCardImports,
    HlmDatePicker,
    BrnSelectImports,
    HlmSelectImports,
  ],

  templateUrl: './contact-info-section.component.html',
})
export class ContactInfoSectionComponent {
  form = input.required<FormGroup>();
  minDate = new Date();
}
