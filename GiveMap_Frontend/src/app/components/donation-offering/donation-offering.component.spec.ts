import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonationOfferingComponent } from './donation-offering.component';

describe('DonationOfferingComponent', () => {
  let component: DonationOfferingComponent;
  let fixture: ComponentFixture<DonationOfferingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonationOfferingComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DonationOfferingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
