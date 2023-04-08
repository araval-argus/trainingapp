import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentChatComponent } from './recent-chat.component';

describe('RecentChatComponent', () => {
  let component: RecentChatComponent;
  let fixture: ComponentFixture<RecentChatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RecentChatComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecentChatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
