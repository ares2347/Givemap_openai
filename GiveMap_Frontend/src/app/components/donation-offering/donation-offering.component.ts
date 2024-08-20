import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-donation-offering',
  templateUrl: './donation-offering.component.html',
  styleUrls: ['./donation-offering.component.scss']
})
export class DonationOfferingComponent implements OnInit {
  @Input() selectedNeed!: any;
  @Input() donations: any[] = [];

  donationForm: FormGroup;
  editingDonationId: string | null = null;
  errorMessage = '';
  successMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private locationService: LocationService,
  ) {
    this.donationForm = this.formBuilder.group({
      description: ['', [Validators.required, Validators.minLength(10)]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      condition: ['new', Validators.required],
      contactInfo: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.donationForm.valid) {
      const donationData = this.donationForm.value;
      this.addDonation(donationData);
    }
  }

  addDonation(donationData: any): void {

    this.locationService.addDonation(this.selectedNeed.id, donationData).subscribe(
      (response) => {
        this.resetForm();
        this.successMessage = 'Donation offer added successfully!';
        this.errorMessage = '';

      },
      (error) => {
        console.error('Error adding donation offer:', error);
        this.errorMessage = 'Failed to add donation offer. Please try again.';
        this.successMessage = '';
      }
    );
    console.log("🚀 ~ DonationOfferingComponent ~ addDonation ~ this.selectedNeed:", this.selectedNeed)
  }

  updateDonation(donationId: string, donationData: any): void {
    this.locationService.updateDonation(this.selectedNeed.locationId, donationId, donationData).subscribe(
      (response) => {
        const index = this.donations.findIndex(donation => donation.id === donationId);
        if (index !== -1) {
          this.donations[index] = response;
        }
        this.resetForm();
        this.successMessage = 'Donation offer updated successfully!';
        this.errorMessage = '';
      },
      (error) => {
        console.error('Error updating donation offer:', error);
        this.errorMessage = 'Failed to update donation offer. Please try again.';
        this.successMessage = '';
      }
    );
  }

  deleteDonation(donationId: string): void {
    if (confirm('Are you sure you want to delete this donation offer?')) {
      this.locationService.deleteDonation(this.selectedNeed.locationId, donationId).subscribe(
        () => {
          this.donations = this.donations.filter(donation => donation.id !== donationId);
          this.successMessage = 'Donation offer deleted successfully!';
          this.errorMessage = '';
        },
        (error) => {
          console.error('Error deleting donation offer:', error);
          this.errorMessage = 'Failed to delete donation offer. Please try again.';
          this.successMessage = '';
        }
      );
    }
  }

  editDonation(donation: any): void {
    this.editingDonationId = donation.id;
    this.donationForm.patchValue({
      itemName: donation.itemName,
      description: donation.description,
      quantity: donation.quantity,
      condition: donation.condition,
      contactInfo: donation.contactInfo
    });
  }

  resetForm(): void {
    this.donationForm.reset({
      itemName: '',
      description: '',
      quantity: 1,
      condition: 'new',
      contactInfo: ''
    });
    this.editingDonationId = null;
  }
}
