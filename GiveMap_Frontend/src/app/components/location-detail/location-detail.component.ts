import { Component, Input, OnInit } from '@angular/core';
import { LocationService } from '../../services/location.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-location-detail',
  templateUrl: './location-detail.component.html',
  styleUrls: ['./location-detail.component.scss'],
  template: `
    <div class="modal-header">
      <h4 class="modal-title">{{ location.name }}</h4>
      <button
        type="button"
        class="close"
        aria-label="Close"
        (click)="activeModal.dismiss('Cross click')"
      >
        <span aria-hidden="true">&times;</span>
      </button>
    </div>
    <div class="modal-body">
      <p>{{ location.description }}</p>
      <!-- Add more location details here -->
    </div>
    <div class="modal-footer">
      <button
        type="button"
        class="btn btn-outline-dark"
        (click)="activeModal.close('Close click')"
      >
        Close
      </button>
    </div>
  `,
})
export class LocationDetailComponent implements OnInit {
  @Input() location: any;
  feedbackForm: FormGroup;
  feedback: any[] = [];
  averageRating: number = 0;
  errorMessage: string = '';
  successMessage: string = '';
  selectedNeed: any = null;

  constructor(
    public activeModal: NgbActiveModal,
    private locationService: LocationService,
    private formBuilder: FormBuilder
  ) {
    this.feedbackForm = this.formBuilder.group({
      comment: ['', [Validators.required, Validators.minLength(3)]],
      rating: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
    });
  }

  ngOnInit(): void {
    this.loadLocationDetails(this.location.id);
  }

  loadLocationDetails(id: string): void {
    this.locationService.getLocationDetails(id).subscribe(
      (data) => {
        this.location = data;
      },
      (error) => {
        console.error('Error loading location details:', error);
        this.errorMessage =
          'Failed to load location details. Please try again.';
      }
    );
  }

  addFeedback(): void {
    if (this.feedbackForm.valid) {
      const comment = this.feedbackForm.get('comment')?.value;
      const rating = this.feedbackForm.get('rating')?.value;
      this.locationService
        .addFeedback(this.location.id, comment, rating)
        .subscribe(
          (response) => {
            this.location.comments.unshift(response);
            this.feedbackForm.reset();
            this.successMessage = 'Feedback added successfully!';
            this.errorMessage = '';
          },
          (error) => {
            console.error('Error adding feedback:', error);
            this.errorMessage = 'Failed to add feedback. Please try again.';
            this.successMessage = '';
          }
        );
    }
  }

  loadLocationFeedback(id: string): void {
    this.locationService.getLocationFeedback(id).subscribe(
      (data) => {
        this.feedback = data.feedback;
        this.averageRating = data.averageRating;
      },
      (error) => {
        console.error('Error loading location feedback:', error);
        this.errorMessage = 'Failed to load feedback. Please try again.';
      }
    );
  }

  getStarArray(rating: number): number[] {
    return Array(Math.round(rating)).fill(0);
  }

  needSelect(result: any): void {
    this.selectedNeed = result;
    console.log('Selected need:', result);
  }
}
