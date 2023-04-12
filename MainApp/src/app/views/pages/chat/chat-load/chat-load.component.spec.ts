import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatLoadComponent } from './chat-load.component';

describe('ChatLoadComponent', () => {
  let component: ChatLoadComponent;
  let fixture: ComponentFixture<ChatLoadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChatLoadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatLoadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
