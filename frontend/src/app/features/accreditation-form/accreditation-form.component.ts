import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-accreditation-form',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './accreditation-form.component.html',
  styleUrls: ['./accreditation-form.component.css'],
})
export class AccreditationFormComponent {
  accreditationForm: FormGroup;
  openDropdowns: { [key: string]: boolean } = {};
  multiSelectValues: { [key: string]: string[] } = {};

  // State options for dropdown
  states = [
    'Alabama',
    'Alaska',
    'Arizona',
    'Arkansas',
    'California',
    'Colorado',
    'Connecticut',
    'Delaware',
    'Florida',
    'Georgia',
    'Hawaii',
    'Idaho',
    'Illinois',
    'Indiana',
    'Iowa',
    'Kansas',
    'Kentucky',
    'Louisiana',
    'Maine',
    'Maryland',
    'Massachusetts',
    'Michigan',
    'Minnesota',
    'Mississippi',
    'Missouri',
    'Montana',
    'Nebraska',
    'Nevada',
    'New Hampshire',
    'New Jersey',
    'New Mexico',
    'New York',
    'North Carolina',
    'North Dakota',
    'Ohio',
    'Oklahoma',
    'Oregon',
    'Pennsylvania',
    'Rhode Island',
    'South Carolina',
    'South Dakota',
    'Tennessee',
    'Texas',
    'Utah',
    'Vermont',
    'Virginia',
    'Washington',
    'West Virginia',
    'Wisconsin',
    'Wyoming',
  ];

  // Contact method options
  contactMethods = ['Phone', 'SMS', 'Email', 'Video Call'];

  // Delegation tasks options
  delegationTasks = [
    'Contact for Complaints/ Reviews',
    'Contact for Billing Matters',
    'Marketing Contact',
  ];

  // Business entity types
  businessEntityTypes = [
    'Corporation',
    'Limited Liability Company (LLC)',
    'Limited Partnership (LP)',
    'S. Corp',
    'Other',
  ];

  // Customer count ranges
  customerRanges = ['1-499', '500-49,999', '50,000-99,999', '1M+'];

  constructor(private fb: FormBuilder) {
    this.accreditationForm = this.fb.group({
      // Business Information
      businessName: ['', Validators.required],
      doingBusinessAs: [''],
      businessAddress: ['', Validators.required],
      businessState: ['', Validators.required],
      businessCity: ['', Validators.required],
      businessZip: ['', Validators.required],

      // Mailing Address
      mailingAddress: [''],
      contactState: [''],
      contactCity: ['', Validators.required],
      contactZip: ['', Validators.required],

      // Contact Information
      primaryBusinessPhone: ['', Validators.required],
      primaryBusinessEmail: ['', [Validators.required, Validators.email]],
      requestQuoteEmail: [''],

      // Primary Contact
      primaryContactName: ['', Validators.required],
      primaryTitle: ['', Validators.required],
      primaryDateOfBirth: ['', Validators.required],
      primaryContactEmail: ['', [Validators.required, Validators.email]],
      primaryContactNumber: ['', Validators.required],
      preferredContactMethod: ['', Validators.required],
      primaryDelegationTasks: ['', Validators.required],

      // Secondary Contact
      secondaryContactName: [''],
      secondaryEmail: [''],
      secondaryPhone: [''],
      secondaryDelegationTasks: [''],

      // Business Details
      website: ['', Validators.required],
      ein: [''],
      stateBusinessLicense: ['', Validators.required],
      professionalLicense: [''],
      businessDescription: ['', Validators.required],
      businessTypes: ['', Validators.required],
      businessEntityType: ['', Validators.required],
      incorporationDetails: [''],
      numberOfEmployees: ['', Validators.required],
      grossAnnualRevenue: ['', Validators.required],
      avgCustomersPerYear: ['', Validators.required],

      // Submission Details
      submittedByName: ['', Validators.required],
      certificationAgreement: [false, Validators.requiredTrue],
    });
  }

  onSubmit() {
    if (this.accreditationForm.valid) {
      console.log('Form submitted:', this.accreditationForm.value);
      // Handle form submission here
      alert('Form submitted successfully!');
    } else {
      console.log('Form is invalid');
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.accreditationForm.controls).forEach((key) => {
      const control = this.accreditationForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.accreditationForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return 'This field is required';
      }
      if (field.errors['email']) {
        return 'Please enter a valid email address';
      }
    }
    return '';
  }

  // Multi-select methods
  toggleDropdown(fieldName: string): void {
    this.openDropdowns[fieldName] = !this.openDropdowns[fieldName];
  }

  isDropdownOpen(fieldName: string): boolean {
    return this.openDropdowns[fieldName] || false;
  }

  getSelectedOptions(fieldName: string): string[] {
    return this.multiSelectValues[fieldName] || [];
  }

  isOptionSelected(fieldName: string, option: string): boolean {
    return this.getSelectedOptions(fieldName).includes(option);
  }

  selectOption(fieldName: string, option: string): void {
    if (!this.multiSelectValues[fieldName]) {
      this.multiSelectValues[fieldName] = [];
    }

    if (!this.multiSelectValues[fieldName].includes(option)) {
      this.multiSelectValues[fieldName].push(option);
      this.updateFormControl(fieldName);
    }
  }

  removeOption(fieldName: string, option: string, event: Event): void {
    event.stopPropagation();
    if (this.multiSelectValues[fieldName]) {
      this.multiSelectValues[fieldName] = this.multiSelectValues[
        fieldName
      ].filter((item) => item !== option);
      this.updateFormControl(fieldName);
    }
  }

  private updateFormControl(fieldName: string): void {
    const control = this.accreditationForm.get(fieldName);
    if (control) {
      control.setValue(this.multiSelectValues[fieldName]);
      control.markAsTouched();
    }
  }
}
