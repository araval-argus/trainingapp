import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewEditFormComponent } from './new-edit-form.component';

describe('NewEditFormComponent', () => {
  let component: NewEditFormComponent;
  let fixture: ComponentFixture<NewEditFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NewEditFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NewEditFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
