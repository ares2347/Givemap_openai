import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonationTrackingComponent } from './donation-tracking.component';

describe('DonationTrackingComponent', () => {
  let component: DonationTrackingComponent;
  let fixture: ComponentFixture<DonationTrackingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonationTrackingComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DonationTrackingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
