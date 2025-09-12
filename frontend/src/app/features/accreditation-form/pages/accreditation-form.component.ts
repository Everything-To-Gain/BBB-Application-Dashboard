import { Component, signal } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { HlmProgress, HlmProgressIndicator } from '@spartan-ng/helm/progress';
import {
  lucideBuilding,
  lucideUsers,
  lucideFileText,
  lucideAward,
  lucideCircleCheck,
} from '@ng-icons/lucide';
import { HlmCard } from '@spartan-ng/helm/card';
import { FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { BusinessInfoSectionComponent } from '../components/business-info-section/business-info-section.component';
import { ContactInfoSectionComponent } from '../components/contact-info-section/contact-info-section.component';
import { BusinessDetailsSectionComponent } from '../components/business-details-section/business-details-section.component';
import { AgreementSectionComponent } from '../components/agreement-section/agreement-section.component';
import { HlmButton } from '@spartan-ng/helm/button';

@Component({
  selector: 'app-accreditation-form',
  imports: [
    ReactiveFormsModule,
    HlmProgress,
    HlmProgressIndicator,
    NgIcon,
    HlmIcon,
    HlmCard,
    HlmButton,
    BusinessInfoSectionComponent,
    ContactInfoSectionComponent,
    BusinessDetailsSectionComponent,
    AgreementSectionComponent,
  ],
  providers: [
    provideIcons({
      lucideBuilding,
      lucideUsers,
      lucideFileText,
      lucideAward,
      lucideCircleCheck,
    }),
  ],
  templateUrl: './accreditation-form.component.html',
})
export class AccreditationFormComponent {
  progress = signal(25);
  currentStep = signal(1);
  steps = [
    { id: 1, title: 'Business Information', icon: 'lucideBuilding' },
    { id: 2, title: 'Contact Information', icon: 'lucideUsers' },
    { id: 3, title: 'Business Details', icon: 'lucideFileText' },
    { id: 4, title: 'Agreement', icon: 'lucideAward' },
  ];

  // Create the form with all fields using FormGroup and FormControl
  accreditationForm: FormGroup = new FormGroup({
    // Business Information
    businessName: new FormControl(''),
    doingBusinessAs: new FormControl(''),
    businessAddress: new FormControl(''),
    businessState: new FormControl(''),
    businessCity: new FormControl(''),
    businessZip: new FormControl(''),
    mailingAddress: new FormControl(''),
    contactState: new FormControl(''),
    contactCity: new FormControl(''),
    contactZip: new FormControl(''),

    // Contact Information
    primaryBusinessPhone: new FormControl(''),
    primaryBusinessEmail: new FormControl(''),
    requestQuoteEmail: new FormControl(''),
    primaryContactName: new FormControl(''),
    primaryTitle: new FormControl(''),
    primaryDateOfBirth: new FormControl(null),
    primaryContactEmail: new FormControl(''),
    primaryContactNumber: new FormControl(''),
    preferredContactMethod: new FormControl([]),
    primaryDelegationTasks: new FormControl([]),
    secondaryContactName: new FormControl(''),
    secondaryEmail: new FormControl(''),
    secondaryPhone: new FormControl(''),
    secondaryDelegationTasks: new FormControl([]),

    // Business Details
    website: new FormControl(''),
    ein: new FormControl(''),
    stateBusinessLicense: new FormControl(''),
    professionalLicense: new FormControl(''),
    businessDescription: new FormControl(''),
    businessTypes: new FormControl(''),
    businessEntityType: new FormControl([]),
    incorporationDetails: new FormControl(''),
    numberOfEmployees: new FormControl(''),
    grossAnnualRevenue: new FormControl(''),
    avgCustomersPerYear: new FormControl([]),
    submittedByName: new FormControl(''),
    trackingEmail: new FormControl(''),

    // Agreement
    agreement: new FormControl(false),
  });

  onSubmit(): void {
    if (this.accreditationForm.valid) {
      console.log('Form submitted:', this.accreditationForm.value);
      // You can add toast notification here
    } else {
      this.accreditationForm.markAllAsTouched();
      console.log('Form is invalid');
    }
  }

  nextStep(): void {
    if (this.currentStep() < this.steps.length) {
      this.currentStep.update((value) => value + 1);
      this.progress.update((value) => value + 25);
    }
  }

  prevStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.update((value) => value - 1);
      this.progress.update((value) => value - 25);
    }
  }

  getStepClasses(stepId: number): string {
    const isCompleted = this.currentStep() > stepId;
    const isCurrent = this.currentStep() === stepId;

    let classes = 'flex flex-col items-center ';

    if (isCurrent) classes += 'text-primary';
    else if (isCompleted) classes += 'text-success';
    else classes += 'text-muted-foreground';
    return classes;
  }

  getStepIconClasses(stepId: number): string {
    const isCompleted = this.currentStep() > stepId;
    const isCurrent = this.currentStep() === stepId;

    let classes = 'rounded-full p-2 mb-2 flex items-center justify-center ';

    if (isCurrent) classes += 'bg-primary text-primary-foreground';
    else if (isCompleted) classes += 'bg-success text-success-foreground';
    else classes += 'bg-muted text-muted-foreground';
    return classes;
  }
}
