import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FriendsDropdownComponent } from './friends-dropdown.component';

describe('FriendsDropdownComponent', () => {
  let component: FriendsDropdownComponent;
  let fixture: ComponentFixture<FriendsDropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FriendsDropdownComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FriendsDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
