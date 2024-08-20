import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {
  filterForm: FormGroup;
  categories: string[] = ['Food', 'Clothing', 'Shelter', 'Medical', 'Education', 'Other'];
  @Output() filterResults = new EventEmitter<any[]>();

  constructor(
    private formBuilder: FormBuilder,
    private locationService: LocationService
  ) {
    this.filterForm = this.formBuilder.group({
      category: [''],
      dateAdded: ['']
    });
  }

  ngOnInit(): void {
    this.filterForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }

  applyFilters(): void {
    const filters = this.filterForm.value;
    this.locationService.filterLocations(filters).subscribe(
      (results) => {
        this.filterResults.emit(results);
      },
      (error) => {
        console.error('Error filtering locations:', error);
        this.filterResults.emit([]);
      }
    );
  }

  resetFilters(): void {
    this.filterForm.reset();
  }
}
