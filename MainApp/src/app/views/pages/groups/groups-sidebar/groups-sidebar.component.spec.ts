import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupsSidebarComponent } from './groups-sidebar.component';

describe('GroupsSidebarComponent', () => {
  let component: GroupsSidebarComponent;
  let fixture: ComponentFixture<GroupsSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupsSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupsSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
