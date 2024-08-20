import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LocationService } from '../../services/location.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-needs-management',
  templateUrl: './needs-management.component.html',
  styleUrls: ['./needs-management.component.scss'],
})
export class NeedsManagementComponent implements OnInit {
  @Input() locationId!: string;
  @Output() needSelect = new EventEmitter<any>();
  needForm: FormGroup;
  editingNeedId: string | null = null;
  errorMessage = '';
  successMessage = '';
  needs: any[] = [];


  constructor(
    private formBuilder: FormBuilder,
    private locationService: LocationService,
  ) {
    this.needForm = this.formBuilder.group({
      description: ['', [Validators.required, Validators.minLength(10)]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      category: [Validators.required],
    });
  }

  ngOnInit(): void {
    this.fetchNeed();
  }

  onSubmit(): void {
    if (this.needForm.valid) {
      const needData = this.needForm.value;
      if (this.editingNeedId) {
        this.updateNeed(this.editingNeedId, needData);
      } else {
        this.addNeed(needData);
      }
    }
  }

  addNeed(needData: any): void {
    this.locationService.addNeed(this.locationId, needData).subscribe(
      (response) => {
        this.needs.push(response);
        this.resetForm();
        this.successMessage = 'Need added successfully!';
        this.errorMessage = '';
      },
      (error) => {
        console.error('Error adding need:', error);
        this.errorMessage = 'Failed to add need. Please try again.';
        this.successMessage = '';
      }
    );
  }

  updateNeed(needId: string, needData: any): void {
    this.locationService
      .updateNeed(this.locationId, needId, needData)
      .subscribe(
        (response) => {
          const index = this.needs.findIndex((need) => need.id === needId);
          if (index !== -1) {
            this.needs[index] = response;
          }
          this.resetForm();
          this.successMessage = 'Need updated successfully!';
          this.errorMessage = '';
        },
        (error) => {
          console.error('Error updating need:', error);
          this.errorMessage = 'Failed to update need. Please try again.';
          this.successMessage = '';
        }
      );
  }

  deleteNeed(needId: string): void {
    if (confirm('Are you sure you want to delete this need?')) {
      this.locationService.deleteNeed(this.locationId, needId).subscribe(
        () => {
          this.needs = this.needs.filter((need) => need.id !== needId);
          this.successMessage = 'Need deleted successfully!';
          this.errorMessage = '';
        },
        (error) => {
          console.error('Error deleting need:', error);
          this.errorMessage = 'Failed to delete need. Please try again.';
          this.successMessage = '';
        }
      );
    }
  }

  editNeed(need: any): void {
    this.editingNeedId = need.id;
    this.needForm.patchValue({
      title: need.title,
      description: need.description,
      quantity: need.quantity,
      urgency: need.urgency,
    });
  }

  resetForm(): void {
    this.needForm.reset({
      title: '',
      description: '',
      quantity: 1,
      urgency: 'medium',
    });
    this.editingNeedId = null;
  }

  fetchNeed(){
    this.locationService.getNeed(this.locationId).subscribe(
      (data) => {
        this.needs = data;
      },
      (error) => {
        console.error('Error loading needs:', error);
        this.errorMessage = 'Failed to load needs. Please try again.';
      }
    );
  }

  selectNeed(need: any): void {
    this.needSelect.emit(need);
  }
}
