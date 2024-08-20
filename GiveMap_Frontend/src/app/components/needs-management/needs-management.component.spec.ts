import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NeedsManagementComponent } from './needs-management.component';

describe('NeedsManagementComponent', () => {
  let component: NeedsManagementComponent;
  let fixture: ComponentFixture<NeedsManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NeedsManagementComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NeedsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
