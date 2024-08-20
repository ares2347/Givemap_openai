import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LocationService } from '../../services/location.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule ],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  searchForm: FormGroup;
  @Output() resultSelected = new EventEmitter<any>();
  searchResults: any[] = [];



  constructor(
    private formBuilder: FormBuilder,
    private locationService: LocationService
  ) {
    this.searchForm = this.formBuilder.group({
      keyword: ['']
    });
  }

  ngOnInit(): void {
    this.searchForm.get('keyword')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(keyword => {
        if (keyword) {
          this.performSearch(keyword);
        } else {
          // this.searchResults.emit([]);
        }
      });
  }

  performSearch(keyword: string): void {
    this.locationService.searchLocations(keyword).subscribe(
      (results) => {
        this.searchResults = results;
      },
      (error) => {
        console.error('Error searching locations:', error);
      }
    );
  }

  onResultClick(result: any) {
    this.resultSelected.emit(result);
    this.searchResults = []; // Clear results after selection
  }
}
