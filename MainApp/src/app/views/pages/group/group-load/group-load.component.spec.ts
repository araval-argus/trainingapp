import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupLoadComponent } from './group-load.component';

describe('GroupLoadComponent', () => {
  let component: GroupLoadComponent;
  let fixture: ComponentFixture<GroupLoadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupLoadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupLoadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
