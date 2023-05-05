import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupContentHeaderComponent } from './group-content-header.component';

describe('GroupContentHeaderComponent', () => {
  let component: GroupContentHeaderComponent;
  let fixture: ComponentFixture<GroupContentHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupContentHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupContentHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
