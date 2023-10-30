import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupchatpageComponent } from './groupchatpage.component';

describe('GroupchatpageComponent', () => {
  let component: GroupchatpageComponent;
  let fixture: ComponentFixture<GroupchatpageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GroupchatpageComponent]
    });
    fixture = TestBed.createComponent(GroupchatpageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
