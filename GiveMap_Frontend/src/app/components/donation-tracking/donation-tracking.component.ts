import { Component, OnInit } from '@angular/core';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-donation-tracking',
  templateUrl: './donation-tracking.component.html',
  styleUrls: ['./donation-tracking.component.scss']
})
export class DonationTrackingComponent implements OnInit {
  donations: any[] = [];
  errorMessage = '';

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    this.loadUserDonations();
  }

  loadUserDonations(): void {
    this.locationService.getUserDonations().subscribe(
      (data) => {
        this.donations = data;
      },
      (error) => {
        console.error('Error loading user donations:', error);
        this.errorMessage = 'Failed to load donations. Please try again.';
      }
    );
  }

  getStatusClass(status: string): string {
    console.log("🚀 ~ DonationTrackingComponent ~ getStatusClass ~ status:", status)
    switch (status.toLowerCase()) {
      case 'offered':
        return 'bg-warning';
      case 'accepted':
        return 'bg-info';
      case 'in transit':
        return 'bg-primary';
      case 'delivered':
        return 'bg-success';
      case 'rejected':
        return 'bg-danger';
      default:
        return 'bg-secondary';
    }
  }
}
