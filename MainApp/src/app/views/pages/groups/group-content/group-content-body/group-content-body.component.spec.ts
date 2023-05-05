import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupContentBodyComponent } from './group-content-body.component';

describe('GroupContentBodyComponent', () => {
  let component: GroupContentBodyComponent;
  let fixture: ComponentFixture<GroupContentBodyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupContentBodyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupContentBodyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
